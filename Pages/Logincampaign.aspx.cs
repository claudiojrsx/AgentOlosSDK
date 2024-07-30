using static OlosAgentSDK.Models.Olos;

using System;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Script.Services;

namespace OlosAgentSDK.Pages
{
    public partial class Logincampaign : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetLoginCampaignIdAtiva(LoginCampaign loginCampaign)
        {
            try
            {
                if (loginCampaign.CampaignName.IndexOf("Ativa", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int campaignId = loginCampaign.CampaignId;
                    Funcoes funcoes = new Funcoes();
                    funcoes.SetSession("CampaignIdAtiva", campaignId);
                    return campaignId.ToString();
                }

                return null;
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }
    }
}