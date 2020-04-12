using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public abstract class FileContentProperty
    {
        private object _value;

        public FileContentProperty()
        {
            Id = Guid.NewGuid();
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Guid Id { get; private set; }

        abstract public void Sync(string filefullpath);
    }
}
