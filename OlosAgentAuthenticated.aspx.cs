using System;
using System.Web;
using System.Web.Script.Services;

namespace OlosAgentSDK
{
    [ScriptService]
    public partial class OlosAgentAuthenticated : System.Web.UI.Page
    {
        private static readonly Funcoes funcoes = new Funcoes();

        private string GetDecodedQueryString(string key)
        {
            string value = Request.QueryString[key];
            return value != null ? funcoes.COBCRPTO(HttpUtility.UrlDecode(value)).Replace("ñ", "1") : string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (!IsPostBack)
            {
                Startup.Session_Start(null, null);
            }

            try
            {
                string agentLogin = GetDecodedQueryString("A");
                string agentPassword = GetDecodedQueryString("B");
                string dbServerIp = GetDecodedQueryString("C");
                string dbName = GetDecodedQueryString("D");
                string dbLogin = GetDecodedQueryString("E");
                string dbPassword = GetDecodedQueryString("F");
                string dbPort = GetDecodedQueryString("G");
                string idUsuario = GetDecodedQueryString("H");

                if (string.IsNullOrEmpty(agentLogin) || string.IsNullOrEmpty(agentPassword) || string.IsNullOrEmpty(idUsuario))
                {
                    Response.Write("Error: Parâmetros inválidos ou ausentes.");
                    return;
                }

                // Definir sessão do usuário
                funcoes.SetSession("idUsuario", idUsuario);

                funcoes.Connection(dbServerIp, dbName, dbLogin, dbPassword, null, null, dbPort);

                var parameters = funcoes.ValoresSQL2($@"
                SELECT  BASE_URL = BB.SERV_AUDIO,
                        SERVIDOR = BB.SERVIDOR,
                        API_TOKEN = BB.SERV_USU,
                        PASSWORD = BB.SERV_SENHA,
                        CLIENTID = BB.DISC_LOGIN,
                        CLIENTSECRET = BB.DISC_SENHA
                FROM    USU_MASTER AA (NOLOCK)
                JOIN    DISC_MASTER BB (NOLOCK) ON BB.ID_DISC_MASTER = AA.ID_DISC_MASTER
                JOIN    USU_DISC CC (NOLOCK) ON CC.ID_DISC_MASTER = BB.ID_DISC_MASTER
                JOIN    DISC_CAD DD (NOLOCK) ON DD.ID_DISCADOR = BB.ID_DISCADOR
                WHERE   DD.DESCR = 'OLOS API'
                AND     AA.ID_USUARIO = {idUsuario}");

                if (parameters.Count == 0)
                {
                    Response.Write("Error: Configurações da API não encontradas.");
                    return;
                }

                string baseURL = parameters["BASE_URL"][0].ToString();
                string apiToken = parameters["API_TOKEN"][0].ToString();
                string password = funcoes.COBCRPTO(parameters["PASSWORD"][0].ToString()).Replace("ñ", "1");
                string clientID = parameters["CLIENTID"][0].ToString();
                string clientSecret = parameters["CLIENTSECRET"][0].ToString();

                funcoes.js($"OlosAgent.setBaseURL('{baseURL}');", this);
                funcoes.js($"OlosAgent.setAuth('{apiToken}', '{password}', '{clientID}', '{clientSecret}');", this);
                funcoes.js($"OlosAgent.authenticatedOlos('{agentLogin}', '{agentPassword}');", this);
            }
            catch (Exception ex)
            {
                Response.Write("Ocorreu um erro ao processar sua solicitação: " + ex.Message);
            }
        }
    }
}
