using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LMS.MVC.UI.Startup))]
namespace LMS.MVC.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
