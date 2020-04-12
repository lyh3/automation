using System;
namespace McAfeeLabs.Engineering.Automation.Base
{
    public interface IFileFilterProvider: IDisposable
    {
        string FileFilter { get; set; }
        ulong? MaxSize { get; set; }
        ulong? MinSize { get; set; }
        string LastErrorMessage { get; }
        bool ShouldFileFilterOut(string file);
    }
}
