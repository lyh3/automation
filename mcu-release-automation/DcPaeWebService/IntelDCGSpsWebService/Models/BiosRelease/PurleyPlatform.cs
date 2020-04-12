using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class PurleyPlatform : PlatformInfo
    {
        public PurleyPlatform():base()
        {
            _platform = SupportedPlatform.Purley;
        }
        protected override void Initialize()
        {
            _releaseList.AddRange(new string[]{
            "Purley_WW31",
            "Purley_WW29",
            });
        }
    }
}