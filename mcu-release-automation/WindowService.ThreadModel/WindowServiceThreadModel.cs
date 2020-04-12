using System;
using System.Collections.Generic;

using Automation.WorkerThreadModel;

namespace Windows.Service
{
    public abstract partial class WindowServiceThreadModel : WindowsService
    {
        #region Declarations

        protected List<IWindowServiceProcess> _serviceThreadProcess = new List<IWindowServiceProcess>();

        #endregion

        #region Constructors

        public WindowServiceThreadModel(String serviceName, string serviceDescription)
            : base(serviceName, serviceDescription)
        {
        }

        #endregion

        #region Properties

        public List<IWindowServiceProcess> ServiceThreadProcess
        {
            get { return _serviceThreadProcess; }
            set { _serviceThreadProcess = value; }
        }

        #endregion

        public override void StartService()
        {
            InitializeWorkerThreads();
        }

        public override void StopService()
        {
        }

        protected override void Dispose(bool disposing)
        {
            _serviceThreadProcess.ForEach(threadProcess => threadProcess.Stop());
            base.Dispose(disposing);
        }

        protected void StartThreadProcess()
        {
            _serviceThreadProcess.ForEach(threadProcess => threadProcess.Start());
        }

        abstract protected void InitializeWorkerThreads();
    }
}
