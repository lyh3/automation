using System;
using System.Collections.Generic;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public interface IInspector : IDisposable
    {
        List<string> ReceivedFiles { get;}
        bool IsComplete { get; }
        bool CanDispose { get; set; }
        int BatchSize { get; set; }
        IEnumerable<string> Inspect();
        event EventHandler BatchResultsForward;
    }
}
