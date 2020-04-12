using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

namespace  McAfeeLabs.Engineering.Automation.Base
{
    public class QueuedBackgroundWorker //: Component
    {
        private Queue<OperationInfo> _operationQueue = new Queue<OperationInfo>();  // Holds pending (possibly canceled) operation requests.
        private Hashtable _userStateToOperationMap = new Hashtable();               // Maps user-supplied keys onto pending OperationInfo.
        private object _collectionsLock = new object();                             // Used to synchronize all access to both of the above two collections.

        private bool _supportsProgress;                                             // Set at construction.  Indicates whether this instance supports calls to ReportProgress.
        private bool _supportsCancellation;                                         // Set at construction.  Indicates whether this instance supports calls to CancelAsync/CancelAllAsync.
        private bool _cancelAllPending = false;

        // DoWork event support.
        private event DoWorkEventHandler _doWork;
        private object _doWorkLock = new object();
        private event ProgressChangedEventHandler _operationProgressChanged;
        private object _operationProgressChangedLock = new object();
        private event RunWorkerCompletedEventHandler _operationCompleted;
        private object _operationCompletedLock = new object();

        public QueuedBackgroundWorker()
            : this(true, true)
        {
        }

        public QueuedBackgroundWorker(bool supportsProgress, bool supportsCancellation)
        {
            _supportsProgress = supportsProgress;
            _supportsCancellation = supportsCancellation;
        }

        #region Property

        public bool SupportsProgressReports
        {
            get { return (_supportsProgress); }
        }

        public bool SupportsCancellation
        {
            get { return (_supportsCancellation); }
        }

        public object DoWorkLock
        {
            get { return _doWorkLock; }
            set { _doWorkLock = value; }
        }

        public DoWorkEventHandler DoWork
        {
            get { return _doWork; }
            set { _doWork = value; }
        }

        public ProgressChangedEventHandler OperationProgressChanged
        {
            get { return _operationProgressChanged; }
            set { _operationProgressChanged = value; }
        }

        public object OperationProgressChangedLock
        {
            get { return _operationProgressChangedLock; }
            set { _operationProgressChangedLock = value; }
        }

        public RunWorkerCompletedEventHandler OperationCompleted
        {
            get { return _operationCompleted; }
            set { _operationCompleted = value; }
        }

        public object OperationCompletedLock
        {
            get { return _operationCompletedLock; }
            set { _operationCompletedLock = value; }
        }

        #endregion

        #region Public Method

        public void RunWorkerAsync(object userState)
        {
            if (userState == null)
            {
                throw new ArgumentNullException("userState cannot be null.");
            }

            int prevCount;
            OperationRequest opRequest = new OperationRequest(userState, OperationHandler);
            OperationInfo opInfo = new OperationInfo(opRequest);

            lock (_collectionsLock)
            {
                if (_userStateToOperationMap.ContainsKey(userState))
                {
                    throw new InvalidOperationException("The specified userKey has already been used to identify a pending operation.  Each userState parameter must be unique.");
                }

                //Make a note of the current pending queue size.  If it's zero at this point,
                //we'll need to kick off an operation.

                prevCount = _operationQueue.Count;

                // Place the new work item on the queue & also in the userState-to-OperationInfo map.
                //
                _operationQueue.Enqueue(opInfo);
                _userStateToOperationMap[userState] = opInfo;
            }

            if (prevCount == 0)
            {
                // We just queued up the first item - kick off the operation.
                //
                opRequest.OperationHandler.BeginInvoke(opInfo, OperationHandlerDone, opInfo);
            }
        }

        public void ReportProgress(int percentComplete, object userState)
        {
            if (!_supportsProgress)
            {
                throw new InvalidOperationException("This instance of the QueuedBackgroundWorker does not support progress notification.");
            }

            OperationInfo opInfo;

            lock (_collectionsLock)
            {
                opInfo = _userStateToOperationMap[userState] as OperationInfo;
            }

            if (opInfo != null)
            {
                RaiseProgressChangedEventFromAsyncContext(percentComplete, userState, opInfo);
            }
        }

        public void CancelAsync(object userState)
        {
            if (!_supportsCancellation)
            {
                throw new InvalidOperationException("This instance of the QueuedBackgroundWorker does not support cancellation.");
            }

            OperationInfo opInfo = GetOperationForUserKey(userState);

            if (opInfo != null)
            {
                opInfo.OperationRequest.Cancel();
            }
        }

        public void CancelAllAsync()
        {
            if (!_supportsCancellation)
            {
                throw new InvalidOperationException("This instance of the QueuedBackgroundWorker does not support cancellation.");
            }

            lock (_collectionsLock)
            {
                _cancelAllPending = true;

                foreach (object key in _userStateToOperationMap.Keys)
                {
                    OperationInfo opInfo = _userStateToOperationMap[key] as OperationInfo;

                    if (opInfo != null)
                    {
                        opInfo.OperationRequest.Cancel();
                    }
                }
            }
        }

        public bool IsCancellationPending(object userState)
        {
            if (!_supportsCancellation)
            {
                return (false);
            }

            if (_cancelAllPending)
            {
                return (true);
            }

            OperationInfo opInfo = GetOperationForUserKey(userState);
            return (opInfo != null ? opInfo.OperationRequest.CancelPending : false);
        }

        #endregion

        #region Private Method

        private void RaiseDoWorkEventFromAsyncContext(DoWorkEventArgs eventArgs)
        {
            Delegate[] targets;

            lock (_doWorkLock)
            {
                targets = _doWork.GetInvocationList();
            }

            foreach (DoWorkEventHandler handler in targets)
            {
                handler(this, eventArgs);
            }
        }

        private void RaiseProgressChangedEventFromAsyncContext(int percentComplete, object userState, OperationInfo opInfo)
        {
            ProgressChangedEventArgs eventArgs = new ProgressChangedEventArgs(percentComplete, userState);
            opInfo.OperationRequest.AsyncOperation.Post(RaiseProgressChangedEventFromClientContext, eventArgs);
        }

        private void RaiseProgressChangedEventFromClientContext(object state)
        {
            ProgressChangedEventArgs eventArgs = (ProgressChangedEventArgs)state;
            Delegate[] targets;

            lock (_operationProgressChangedLock)
            {
                targets = _operationProgressChanged.GetInvocationList();
            }

            foreach (ProgressChangedEventHandler handler in targets)
            {
                try
                {
                    handler(this, eventArgs);
                }
                catch
                {
                }
            }
        }

        private void RaiseWorkCompletedEventFromAsyncContext(OperationInfo opInfo)
        {
            opInfo.OperationRequest.AsyncOperation.PostOperationCompleted(RaiseWorkCompletedEventFromClientContext, opInfo);
        }

        private void RaiseWorkCompletedEventFromClientContext(object state)
        {
            OperationInfo opInfo = (OperationInfo)state;
            RunWorkerCompletedEventArgsWithUserState eventArgs = opInfo.OperationResult;
            Delegate[] targets;

            lock (_operationCompletedLock)
            {
                targets = _operationCompleted.GetInvocationList();
            }

            foreach (RunWorkerCompletedEventHandler handler in targets)
            {
                try
                {
                    handler(this, eventArgs);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("---{0}:RaiseWorkCompletedEventFromClientContext, exception caught : {1}", this.ToString(), ex.Message));
                    //throw ex;
                }
            }

            // Now that we're done calling back to the client to let them know
            // that the operation has completed, remove this operation from the
            // queue and check to see if we need to start another operation.
            //
            OperationRequest opRequest = opInfo.OperationRequest;
            OperationInfo nextOp = null;

            lock (_collectionsLock)
            {
                if ((_operationQueue.Peek() != opInfo) || !_userStateToOperationMap.ContainsKey(opRequest.UserState))
                {
                    throw new InvalidOperationException("Something freaky happened.");
                }

                _operationQueue.Dequeue();
                _userStateToOperationMap.Remove(opInfo);

                if (_operationQueue.Count > 0)
                {
                    nextOp = _operationQueue.Peek();
                }
                else
                {
                    _cancelAllPending = false;
                }
            }

            if (nextOp != null)
            {
                // We have more work items pending.  Kick off another operation.
                //
                nextOp.OperationRequest.OperationHandler.BeginInvoke(nextOp, OperationHandlerDone, nextOp);
            }
        }

        private OperationInfo GetOperationForUserKey(object userKey)
        {
            lock (_collectionsLock)
            {
                return (_userStateToOperationMap[userKey] as OperationInfo);
            }
        }

        private RunWorkerCompletedEventArgsWithUserState OperationHandler(OperationInfo opInfo)
        {
            object userState = opInfo.OperationRequest.UserState;
            DoWorkEventArgs eventArgs = new DoWorkEventArgs(userState);
            
            try
            {
                RaiseDoWorkEventFromAsyncContext(eventArgs);

                if (eventArgs.Cancel)
                {
                    opInfo.OperationRequest.Cancel(); // For the sake of completeness.
                    return new RunWorkerCompletedEventArgsWithUserState(null, null, true, userState);
                }
                else
                {
                    return new RunWorkerCompletedEventArgsWithUserState(eventArgs.Result, null, false, userState);
                }
            }
            catch( Exception err )
            {
                return new RunWorkerCompletedEventArgsWithUserState(null, err, false, userState);
            }
        }

        private void OperationHandlerDone(IAsyncResult ar)
        {
            OperationInfo opInfo = (OperationInfo)ar.AsyncState;
            opInfo.OperationResult = opInfo.OperationRequest.OperationHandler.EndInvoke(ar);
            RaiseWorkCompletedEventFromAsyncContext(opInfo);
        }

        #endregion

    }

}
