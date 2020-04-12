using System;
using System.ServiceProcess;
using System.Threading;

using Windows.Service;

namespace WindowsServiceBaseMock {
    public partial class WindowsServiceBaseMock : WindowsService
    {
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private Thread _inputThread;

        public WindowsServiceBaseMock(string serviceName, string serviceDescription, ServiceStartMode startType = ServiceStartMode.Automatic)
            : base(serviceName, serviceDescription, ServiceStartMode.Automatic) {
                InputThread _input = new InputThread(Logger, _stopEvent);
                _inputThread = new Thread(new ThreadStart(_input.DoWork));
        }

        public override void StartService() {
            _inputThread.Start();
        }
        public override void StopService() {
            _stopEvent.Set();
            _inputThread.Join();
        }
    }
}
