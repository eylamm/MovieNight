using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieNight.Startup))]
namespace MovieNight
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
