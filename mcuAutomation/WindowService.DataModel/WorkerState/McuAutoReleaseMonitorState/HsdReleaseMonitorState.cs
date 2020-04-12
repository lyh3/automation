using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using Automation.WorkerThreadModel;

namespace WindowService.DataModel
{
    abstract public class HsdReleaseMonitorState : WorkerState
    {
        public HsdReleaseMonitorState(WorkerThread parent, ILog logger) : base(parent, logger) { }
        override public WorkerThread ParentThread
        {
            get { return _parentThread as McuReleaseMonitorWorkerThread; }
        }
    }
}
