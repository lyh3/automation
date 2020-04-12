using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class WhitleyPlatform : PlatformInfo
    {
        public WhitleyPlatform():base()
        {
            _platform = SupportedPlatform.Whitley;
        }
        protected override void Initialize()
        {
            _releaseList.AddRange(new string[]{
            "Whitley_WW31",
            "Whitley_WW29",
            });
        }
    }
}