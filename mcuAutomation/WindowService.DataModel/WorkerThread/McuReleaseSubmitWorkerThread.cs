using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using log4net;
using Automation.WorkerThreadModel;

namespace WindowService.DataModel
{
    public class McuReleaseSubmitWorkerThread : WorkerThread
    {
        private string _request = string.Empty;
        private string _automationDir = string.Empty;
        private string _workspace = string.Empty;
        private string _invokeParameters = string.Empty;
        private string _releaseSource = string.Empty;
        private Queue<WorkerState> _queue = null;
        public McuReleaseSubmitWorkerThread(ILog logger) : base(logger)
        {
        }
        public string Request
        {
            get { return _request; }
            set { _request = value; }
        }
        public string AutomationDir
        {
            get { return _automationDir; }
            set { _automationDir = value; }
        }
        public string Workspace
        {
            get { return _workspace; }
            set { _workspace = value; }
        }
        public string InvokeParameters
        {
            get { return _invokeParameters; }
            set { _invokeParameters = value; }
        }
        public string ReleaseSource
        {
            get { return _releaseSource; }
            set { _releaseSource = value; }
        }
        public override WorkerState StateFactory(WorkerState args = null)
        {
            WorkerState state = null;
            if (null == args)
            {
                state = InitialWork();
            }
            else
            {
                if (!(args as McuSubmitState).Success)
                {
                    _queue.Clear();
                }
                else if (_queue.Count > 0)
                {
                    state = _queue.Dequeue();
                }
            }
            if(null == state)
            {
                this.Stop();
            }
            return state;
        }
        protected override WorkerState InitialWork()
        {
            WorkerState state = null; 
             _queue = new Queue<WorkerState>();
            _queue.Enqueue(new InvokeWorkflowState(this, _logger));
            _queue.Enqueue(new TearDownState(this, _logger));
            state = _queue.Dequeue();
            return state;
        }
    }
}
