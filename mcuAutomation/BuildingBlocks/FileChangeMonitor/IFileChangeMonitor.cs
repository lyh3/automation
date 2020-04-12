using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public interface IFileChangeMonitor : INotifyPropertyChanged
    {
        FileSystemWatcher Watcher { get; }
    }
}
