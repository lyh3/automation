using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;
using Automation.WorkerThreadModel;
using Automation.Base.BuildingBlocks;

namespace IntelDCGSpsWebService.Models
{
    abstract public class BiosReleaseState : WorkerState
    {
        protected TransactionStatus _transactionStatus = TransactionStatus.Idle;
        public BiosReleaseState(WorkerThread parent, ILog logger) : base(parent, logger) { }
        public string StateTag
        {
            get
            {
                return this.GetType().ToString().Replace("IntelDCGSpsWebService.Models.BiosRelease_", string.Empty).Replace("_State", string.Empty); 
            }
        }
        public string TranStatus
        {
            get { return _transactionStatus.ToString(); }
            set
            {
                _transactionStatus = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), value);
            }
        }
        public string NotificationMessages { get; set; }
    }
}