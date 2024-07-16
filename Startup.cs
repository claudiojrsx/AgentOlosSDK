using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OlosAgentSDK.Startup))]
namespace OlosAgentSDK
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
