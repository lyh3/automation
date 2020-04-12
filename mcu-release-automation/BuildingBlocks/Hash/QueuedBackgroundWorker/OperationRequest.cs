using System;
using System.ComponentModel;
using System.Text;

namespace  McAfeeLabs.Engineering.Automation.Base
{
    /// <summary>
    /// This class represents everything this component needs to know about
    /// in order to carry out a single operation as requested by a call to
    /// RunWorkerAsync.
    /// </summary>
    public class OperationRequest
    {
        internal readonly object UserState;
        internal readonly AsyncOperation AsyncOperation;
        internal readonly OperationHandlerDelegate OperationHandler;
        bool _cancelPending = false;

        internal OperationRequest(object userState, OperationHandlerDelegate operationHandler)
        {
            UserState = userState;
            OperationHandler = operationHandler;
            AsyncOperation = AsyncOperationManager.CreateOperation(this);
        }

        internal bool CancelPending
        {
            get { return (_cancelPending); }
        }

        internal void Cancel()
        {
            _cancelPending = true;
        }
    }
}
