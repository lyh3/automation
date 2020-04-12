using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    public class PropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; set; }
        public dynamic OldValue { get; set; }
        public dynamic NewValue { get; set; }

        public PropertyChangedEventArgs(string propertyName, dynamic oldValue, dynamic newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
