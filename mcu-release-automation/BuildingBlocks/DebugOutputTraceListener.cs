// Original source : http://stackoverflow.com/questions/4429254/how-to-make-debugview-work-under-net-4
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class DebugOutputTraceListener : TraceListener
    {
        #region Declarations

        private IPEndPoint _ipEndPoint;
        private bool _needsDisposing;

        #endregion

        #region Constructor

        public DebugOutputTraceListener(string debugOutputListenerPath, int port)
        {
            this._ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13000);

            var psi = new ProcessStartInfo()
            {
                FileName = debugOutputListenerPath,
                Arguments = port.ToString(),
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(psi);
            _needsDisposing = true;

            Trace.AutoFlush = true;
        }

        ~DebugOutputTraceListener()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        public static void StartListener()
        {
            Debug.Listeners.Add(new DebugOutputTraceListener("DebugOutputListener.exe", 13000));
            Trace.Listeners.Add(new DebugOutputTraceListener("DebugOutputListener.exe", 13001));
        }

        public override void Write(string message)
        {
            sendMessage(message);
        }

        public override void WriteLine(string message)
        {
            sendMessage(message + Environment.NewLine);
        }

        public override void Close()
        {
            sendCloseMessage();
            _needsDisposing = false;
            base.Close();
        }

        #endregion

        #region Private Methods

        protected string EnvironmentInfo 
        { 
            get 
            {
                return string.Format(@"[Address:{0}, Host:{1}]", null == _ipEndPoint ? string.Empty : _ipEndPoint.ToString(), Environment.MachineName);
            }
        }

       private void sendMessage(string message)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(_ipEndPoint);
                    byte[] bufferMessage = Encoding.UTF8.GetBytes(message);
                    byte[] bufferLength =
                        BitConverter.GetBytes(bufferMessage.Length);

                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(bufferLength, 0, bufferLength.Length);
                        stream.Write(bufferMessage, 0, bufferMessage.Length);
                    }
                }
            }
            catch (SocketException e)
            {
                Trace.WriteLine(e.ToString().StringConcat(EnvironmentInfo, front: true));
            }
        }

        /// <summary>
        /// Sends -1 to close the TCP listener server.
        /// </summary>
        private void sendCloseMessage()
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(_ipEndPoint);
                    byte[] buffer = BitConverter.GetBytes(-1);

                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (SocketException e)
            {
                Trace.WriteLine(e.ToString().StringConcat(EnvironmentInfo, front:true));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_needsDisposing)
            {
                sendCloseMessage();
                _needsDisposing = false;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
