using Newtonsoft.Json;
using OlosAgentSDK.Api;
using System;
using System.IO;

namespace OlosAgentSDK.Pages
{
    public partial class SendManualCallRequest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";

            if (Request.HttpMethod == "POST")
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    string jsonData = reader.ReadToEnd();

                    var requestData = JsonConvert.DeserializeObject<ManualCallRequestHandler>(jsonData);

                    var response = new
                    {
                        requestData.AgentId,
                        requestData.Ddd,
                        requestData.PhoneNumber,
                        requestData.CampaignId,
                        Mensagem = "Parâmetros recebidos e processados com sucesso!"
                    };

                    string jsonResponse = JsonConvert.SerializeObject(response);

                    Response.ContentType = "application/json";

                    Response.Write(jsonResponse);
                    Response.End();
                }
            }
        }
    }
}
