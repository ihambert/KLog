using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KLog.Startup))]
namespace KLog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
