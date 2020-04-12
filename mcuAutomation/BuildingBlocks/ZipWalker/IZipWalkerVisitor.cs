using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public interface IZipWalkerVisitor
    {
        void Visit(List<string> files);
    }
}
