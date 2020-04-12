namespace McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy
{
    public interface IDynamicProxy
    {
        dynamic ServiceProxy { get; }
        bool IsServiceAvailable { get; }
    }
}
