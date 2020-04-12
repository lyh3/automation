using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;
using Automation.WorkerThreadModel;
using Automation.Base.BuildingBlocks;

namespace IntelDCGSpsWebService.Models
{
    public class BiosReleaseOrchestrationThread : WorkerThread
    {
        private Queue<WorkerState> _queue = null;
        private BiosReleaseState _currentActiveState = null;
        public BiosReleaseOrchestrationThread(ILog logger) : base(logger) { }

        public override WorkerState StateFactory(WorkerState args = null)
        {
            WorkerState state = null;
            if(null == args)
            {
                state = InitialWork();
            }
            else
            {
                if (!(args as BiosReleaseState).Success)
                {
                    _queue.Clear();
                    _currentActiveState = args as BiosReleaseState;
                    _currentActiveState.TranStatus = TransactionStatus.Failed.ToString();
                }
                else if (_queue.Count > 0)
                {
                    state = _queue.Dequeue();
                }
            }

            if (null != state)
            {
                _currentActiveState = state as BiosReleaseState;
                _currentActiveState.TranStatus = TransactionStatus.Progress.ToString();
            } 
            else
            {
                Stop();
            }
            return state;
        }

        public BiosReleaseState CurrentTransaction 
        { 
            get 
            {
                return _currentActiveState ;
            }
        }

        public void Reset()
        {
            Stop();
            _queue.Clear();
        }

        protected override WorkerState InitialWork()
        {
            WorkerState state = null;
            _queue = new Queue<WorkerState>();
            _queue.Enqueue(new BiosRelease_GenerateReleaseNote_State(this, _logger));
            _queue.Enqueue(new BiosRelease_UpdateSandbox_State(this, _logger));
            _queue.Enqueue(new BiosRelease_CommitStaggingRepo_State(this, _logger));
            state = _queue.Dequeue();
            return state;
        }
    }
}