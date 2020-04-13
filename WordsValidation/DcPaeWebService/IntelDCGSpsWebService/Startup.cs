using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IntelDCGSpsWebService.Startup))]
namespace IntelDCGSpsWebService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
