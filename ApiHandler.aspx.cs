using System;

namespace OlosAgentSDK
{
    public partial class ApiHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";

            try
            {
                string parametro1 = Request.QueryString["param1"];
                string parametro2 = Request.QueryString["param2"];
                
                var handler = new ParameterHandler(parametro1, parametro2);

                string jsonResponse = handler.ToJson();
                Response.Write(jsonResponse);
            }
            catch (ArgumentException ex)
            {
                Response.StatusCode = 400;
                Response.Write("{\"Mensagem\": \"" + ex.Message + "\"}");
            }

            Response.End();
        }
    }
}
