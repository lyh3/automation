using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class BackervillePlatform : PlatformInfo
    {
        public BackervillePlatform(): base()
        {
            _platform = SupportedPlatform.Backerville;
        }
        protected override void Initialize()
        {
            _releaseList.AddRange(new string[]{
            "Backerville_WW31",
            "Backerville_WW29",
            });
        }
    }
}