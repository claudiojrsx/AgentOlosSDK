using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System.Web;
using System;

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

        public static void Session_Start(object sender, EventArgs e)
        {
            string userIP = HttpContext.Current.Request.UserHostAddress;
            string sessionID = HttpContext.Current.Session.SessionID;

            if (userConnections.ContainsKey(userIP))
            {
                userConnections[userIP]++;
            }
            else
            {
                userConnections[userIP] = 1;
            }
        }

        public static void Session_End(object sender, EventArgs e)
        {
            string userIP = HttpContext.Current.Request.UserHostAddress;
            string sessionID = HttpContext.Current.Session.SessionID;

            if (userConnections.ContainsKey(userIP))
            {
                userConnections[userIP]--;
                if (userConnections[userIP] <= 0)
                {
                    userConnections.Remove(userIP);
                }
            }
        }

        public static Dictionary<string, int> GetActiveConnections()
        {
            return userConnections;
        }
    }
}
