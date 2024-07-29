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
            Funcoes funcoes = new Funcoes();

            try
            {
                int agentId = loginCampaign.AgentId;
                int campaignId = loginCampaign.CampaignId;
                string campaignName = loginCampaign.CampaignName;
                string campaignCode = loginCampaign.CampaignCode;

                if (loginCampaign.CampaignName.Contains("Ativa"))
                {
                    funcoes.SetSession("CampaignId", campaignId);
                    return campaignId.ToString();
                }
                else
                {
                    return funcoes.GetSession("CampaignId", "0").ToString();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }
    }
}