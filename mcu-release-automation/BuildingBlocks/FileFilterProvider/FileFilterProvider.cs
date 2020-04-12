namespace McAfeeLabs.Engineering.Automation.Base
{
    abstract public class FileFilterProvider : IFileFilterProvider
    {
        #region Declarations

        protected object _syncObj = new object();
        protected string _lastErrorMessage;

        #endregion

        #region Properties

        public string FileFilter { get; set; }
        public ulong? MaxSize { get; set; }
        public ulong? MinSize { get; set; }
        public string LastErrorMessage { get { return _lastErrorMessage; } }

        #endregion

        #region Public Methods

        abstract public bool ShouldFileFilterOut(string filepath);
        abstract public void Dispose();

        #endregion
    }
}
