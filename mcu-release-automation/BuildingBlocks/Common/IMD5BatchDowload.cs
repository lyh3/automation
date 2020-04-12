using System;
using System.Collections.Generic;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public interface IMD5BatchDowload
    {
        string Uri { get; set; }
        StringBuilder ErrorMessages { get; set; }
        List<string> CurrentBatcMD5hList { get; }
        void SampleBatchDownload(List<string> md5List, string targetfolder, string extenion = @"zip", int batchSize = 5);
        event EventHandler BatchComplete;
    }
}
