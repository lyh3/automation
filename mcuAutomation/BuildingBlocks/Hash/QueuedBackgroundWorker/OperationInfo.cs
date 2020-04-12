using System;
using System.Collections.Generic;
using System.Text;

namespace  McAfeeLabs.Engineering.Automation.Base
{
    public delegate RunWorkerCompletedEventArgsWithUserState OperationHandlerDelegate(OperationInfo opInfo);
    /// <summary>
    /// This class combines a request (OperationRequest) with a result
    /// (RunWorkerCompletedEventArgsWithUserState), and is what's queued up
    /// and processed by this component.
    /// </summary>
    public class OperationInfo
    {
        OperationRequest _request;
        RunWorkerCompletedEventArgsWithUserState _result;

        internal OperationInfo(OperationRequest request)
        {
            _request = request;
            _result = null;
        }

        internal OperationRequest OperationRequest
        {
            get { return (_request); }
        }

        internal RunWorkerCompletedEventArgsWithUserState OperationResult
        {
            get
            {
                if (_result == null)
                {
                    throw new InvalidOperationException("The operation result has not been set yet.");
                }

                return (_result);
            }

            set
            {
                if (_result != null)
                {
                    throw new InvalidOperationException("The operation result has already been set.");
                }

                _result = value;
            }
        }
    }
}
