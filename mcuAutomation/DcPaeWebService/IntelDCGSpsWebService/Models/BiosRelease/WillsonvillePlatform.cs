using System;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class WillsonvillePlatform : PlatformInfo
    {
        public WillsonvillePlatform() : base()
        {
            _platform = SupportedPlatform.Willsonville;
        }
        protected override void Initialize()
        {
            _releaseList.AddRange(new string[]{
            "Willsonville_WW31",
            "Willsonville_WW29",
            });
        }
    }
}