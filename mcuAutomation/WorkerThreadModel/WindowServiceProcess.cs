using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

namespace Automation.WorkerThreadModel
{
    abstract public class WindowServiceProcess : IWindowServiceProcess
    {
        protected ILog _logger;
        protected List<WorkerThread> _workerList = new List<WorkerThread>();
        public WindowServiceProcess() { }
        public WindowServiceProcess(ILog logger):this()
        {
            _logger = logger;
        }
        virtual public void Start()
        {
            _workerList.ForEach(workerthread => workerthread.Start());
        }
        virtual public void Stop()
        {
            _workerList.ForEach(workerthread => workerthread.Stop());
        }
        abstract protected void InitializeWorkerProcess();
    }
}
