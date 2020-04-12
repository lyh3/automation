using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class MehlowPlatform : PlatformInfo
    {
        public MehlowPlatform(): base()
        {
            _platform = SupportedPlatform.Mehlow;
        }
        protected override void Initialize()
        {
            _releaseList.AddRange(new string[]{
            "Mehlow_WW31",
            "Mehlow_WW29",
            });
        }
    }
}