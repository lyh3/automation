using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace McAfeeLabs.Engineering.Automation.Base.FileLogListener
{
    [Serializable]
    public class ConsoleLogListener : FileLogListener
    {
        #region Constructor

        protected ConsoleLogListener(string logFilename) : base(logFilename)
        {
            AllocConsole();
        }

        ~ConsoleLogListener()
        {
            Dispose();
        }

        #endregion

        #region Public Methods

        public override void Write(string message)
        {
            base.Write(message);
            Console.WriteLine(message);
        }

        override public void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            base.WriteLine(message);
            Console.WriteLine(string.Format(@"{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
        }

        static public FileLogListener AddConsoleLogListener(object client)
        {
            if (null == client) return null;
            return AddListner(client.GetType().Name);
        }

        static public FileLogListener AddConsoleLogListener(Type type)
        {
            return AddListner(type.Name);
        }

        static public FileLogListener AddConsoleLogListener(string typeName)
        {
            return AddListner(typeName);
        }

        #endregion

        #region Private Methods

        private static FileLogListener AddListner(string logFilename)
        {
            var fileLogListener = new ConsoleLogListener(logFilename);
            Trace.Listeners.Add(fileLogListener);
            return fileLogListener;
        }
        
        #endregion

        #region DllImport

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        #endregion
    }
}
