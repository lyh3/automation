namespace McAfeeLabs.Engineering.Automation.WorkerThreadModel
{
    public interface IWorkerThread
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
        WorkerState StateFactory(WorkerState args = null);
    }
}
