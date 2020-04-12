using System;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    public delegate void PropertyChangedEventHandler(object source, EventArgs args);

    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChangedEvent(object source, EventArgs args);
    }
}
