using System;
using System.Web;
using System.Web.Script.Services;

namespace OlosAgentSDK
{
    [ScriptService]
    public partial class OlosAgentAuthenticated : System.Web.UI.Page
    {
        private static readonly Funcoes funcoes = new Funcoes();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            try
            {
                string agentLogin = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["A"])).Replace("ñ", "1");
                string agentPassword = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["B"])).Replace("ñ", "1");
                string dbServerIp = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["C"])).Replace("ñ", "1");
                string dbName = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["D"])).Replace("ñ", "1");
                string dbLogin = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["E"])).Replace("ñ", "1");
                string dbPassword = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["F"])).Replace("ñ", "1");
                string dbPort = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["G"])).Replace("ñ", "1");
                string idUsuario = funcoes.COBCRPTO(HttpUtility.UrlDecode(Request.QueryString["H"])).Replace("ñ", "1");

                funcoes.SetSession("idUsuario", idUsuario);

                funcoes.Connection(
                    dbServerIp,
                    dbName,
                    dbLogin,
                    dbPassword,
                    null,
                    null,
                    dbPort);

                if (!string.IsNullOrEmpty(agentLogin) && !string.IsNullOrEmpty(agentPassword))
                {
                    var parameters = funcoes.ValoresSQL2($@"
                    SELECT	BASE_URL = BB.SERV_AUDIO,
                            SERVIDOR = BB.SERVIDOR,
		                    API_TOKEN = BB.SERV_USU,
		                    PASSWORD = BB.SERV_SENHA,
		                    CLIENTID = BB.DISC_LOGIN,
		                    CLIENTSECRET = BB.DISC_SENHA
                    FROM	USU_MASTER AA (NOLOCK)
                    JOIN	DISC_MASTER BB (NOLOCK) ON BB.ID_DISC_MASTER = AA.ID_DISC_MASTER
                    JOIN	USU_DISC CC (NOLOCK) ON CC.ID_DISC_MASTER = BB.ID_DISC_MASTER
                    JOIN	DISC_CAD DD (NOLOCK) ON DD.ID_DISCADOR = BB.ID_DISCADOR
                    WHERE	DD.DESCR = 'OLOS API'
                    AND		AA.ID_USUARIO = {idUsuario}");

                    string baseURL = parameters["BASE_URL"][0].ToString();
                    string apiToken = parameters["API_TOKEN"][0].ToString();
                    string password = funcoes.COBCRPTO(parameters["PASSWORD"][0].ToString()).Replace("ñ", "1");
                    string clientID = parameters["CLIENTID"][0].ToString();
                    string clientSecret = parameters["CLIENTSECRET"][0].ToString();

                    funcoes.js($"OlosAgent.setBaseURL('{baseURL}');", this);
                    funcoes.js($"OlosAgent.setAuth('{apiToken}', '{password}', '{clientID}', '{clientSecret}');", this);
                    funcoes.js($"OlosAgent.authenticatedOlos('{agentLogin}', '{agentPassword}');", this);
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Error: {ex.Message}");
            }
        }
    }
}