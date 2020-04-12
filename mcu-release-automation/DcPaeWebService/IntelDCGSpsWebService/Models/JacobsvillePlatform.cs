using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class JacobsvillePlatform : PlatformInfo
    {
        public JacobsvillePlatform(): base()
        {
            _platform = SupportedPlatform.Jacobsville;
        }
        protected override void Initialize()
        {
            _releaseList.AddRange(new string[]{
            "JocobsVille_WW31",
            "Jacobsville_WW29",
            });
        }
    }
}