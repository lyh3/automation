using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Linq;
using System.Diagnostics;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    abstract public class FileChangeMonitor : IFileChangeMonitor
    {
        #region Declarations

        [ThreadStatic]
        protected static FileSystemWatcher _watcher;
        protected string _filepath;
        protected List<FileContentProperty> _filecontentpropertylist = new List<FileContentProperty>();
        protected bool _lazyupdate = true;
        protected bool _isDirty = false;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor 
        public FileChangeMonitor() { }

        public FileChangeMonitor(string filepath)
        {
            InitializeWatcher(filepath);
        }

        public FileChangeMonitor(string filepath, string filter)
            :this(filepath)
        {
            _watcher.Filter = filter;
        }

        #endregion

        #region Indexer

        public object this[Guid id]
        {
            get
            {
                var property = _filecontentpropertylist.FirstOrDefault<FileContentProperty>(p => p.Id == id);
                if (null != property)
                    return property.Value;
                else
                    return null;
            }
        }

        #endregion

        #region Properties

        public FileSystemWatcher Watcher { get { return _watcher; } }
        public bool LazyUpdate
        {
            get { return _lazyupdate; }
            set { _lazyupdate = value; }
        }
        public string FilePath
        {
            get { return _filepath; }
            set { _filepath = value; if (null != _watcher && _watcher.Path != value) InitializeWatcher(value); }
        }
        
        #endregion


        #region Public Methods

        public void RequestUpdate()
        {
            RaisePropertyChangedEvent(this, null);
        }

        public void RaisePropertyChangedEvent(object source, EventArgs args)
        {
            try
            {
                if (_isDirty && null != PropertyChanged)
                    PropertyChanged(source, args);
            }
            catch (Exception ex) { Trace.WriteLine(string.Format(@"--- Exception caught at FileChangeMonitor, RaisePropertyChangedEvent, error was : {0}", ex.Message)); }
            finally
            {
                _isDirty = false;
            }
        }

        public void AddRange(IEnumerable<FileContentProperty> properties)
        {
            var itr = properties.GetEnumerator();
            while(itr.MoveNext())
                Add(itr.Current);
        }

        public void Add(FileContentProperty property)
        {
            if (!_filecontentpropertylist.Contains(property))
                _filecontentpropertylist.Add(property);
        }

        public void Remove(FileContentProperty property)
        {
            _filecontentpropertylist.Remove(property);
        }

        #endregion

        #region Private Methods

        private void InitializeWatcher(string filepath)
        {
            _watcher = new FileSystemWatcher
            {
                Path = filepath,
                EnableRaisingEvents = true
            };
            _watcher.Changed += OnChanged;
         }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                _filecontentpropertylist.ForEach(x => x.Sync(e.FullPath));

                if (!_lazyupdate)
                    RequestUpdate();

                NotifyChange(e);

            }
            catch (Exception ex) { Trace.WriteLine(string.Format(@"--- Exception caught at FileChangeMonitor, OnChange, error was : {0}", ex.Message)); }
            finally
            {
                _isDirty = true;
            }
        }
        
        abstract protected void NotifyChange(FileSystemEventArgs e);

        #endregion
    }
}
