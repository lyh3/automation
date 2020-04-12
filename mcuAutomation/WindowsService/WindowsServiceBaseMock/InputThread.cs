using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using log4net;

namespace WindowsServiceBaseMock {
    public class InputThread {
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private long _number = 0;
        protected static ILog _logger;

        public InputThread(ILog logger, ManualResetEvent stopEvent) {
            _logger = logger;
            _stopEvent = stopEvent;
        }
        public void DoWork() {
            for (; ; ) {
                if (_stopEvent.WaitOne(1 * 1000, false)) {
                    break;
                } else {
                    long __square = _number * _number;
                    _logger.Error("Number = " + _number.ToString() + " Square=" + __square.ToString());
                    _number++;
                }
            }
        }
    }
}
