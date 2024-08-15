using Microsoft.Owin;
using Owin;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(OlosAgentSDK.Startup))]
namespace OlosAgentSDK
{
    public class Startup
    {
        private static readonly Dictionary<string, int> userConnections = new Dictionary<string, int>();

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
