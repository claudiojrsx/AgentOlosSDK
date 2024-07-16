using OlosAgentSDK.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using static OlosAgentSDK.Models.Olos;

namespace OlosAgentSDK
{
    [ScriptService]
    public partial class OlosAgentAuthenticated : System.Web.UI.Page
    {
        readonly Funcoes funcoes = new Funcoes();
        string sql = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            QueryStrings.QueryStringParameters queryStringParameters = new QueryStrings.QueryStringParameters
            {
                agentLogin = Request.QueryString["agentLogin"],
                agentPassword = Request.QueryString["agentPassword"],
                idUsuario = Request.QueryString["idUsuario"],
                dbServerIp = Request.QueryString["dbServerIp"],
                dbName = Request.QueryString["dbName"],
                dbLogin = Request.QueryString["dbLogin"],
                dbPassword = Request.QueryString["dbPassword"],
                dbPort = Request.QueryString["dbPort"]
            };

            funcoes.SetSession("idUsuario", Request.QueryString["idUsuario"]);

            funcoes.Connection(
                queryStringParameters.dbServerIp, 
                queryStringParameters.dbName, 
                queryStringParameters.dbLogin,
                queryStringParameters.dbPassword, 
                null, 
                null,
                queryStringParameters.dbPort);

            if (!IsPostBack)
            {
                DropDownList();
            }

            if (!string.IsNullOrEmpty(queryStringParameters.agentLogin) && !string.IsNullOrEmpty(queryStringParameters.agentPassword))
            {
                funcoes.js($"OlosAgent.authenticatedOlos('{queryStringParameters.agentLogin}', '{queryStringParameters.agentPassword}');", this);
            }
        }

        protected void DropDownList()
        {
            string  ID_DISC_MASTER = funcoes.ValorSQL($@"SELECT	AA.ID_DISC_MASTER
            FROM	DISC_MASTER AA (NOLOCK)
            JOIN	DISC_CAD BB (NOLOCK) ON BB.ID_DISCADOR = AA.ID_DISCADOR
            WHERE	BB.DESCR = 'OLOS API'");

            sql = $@"SELECT ID_DISC_PAUSA, DESCR FROM DISC_PAUSA WHERE ID_DISC_MASTER = {ID_DISC_MASTER}";
            funcoes.LoadCombo(ddlPausas, sql, "ID_DISC_PAUSA", "DESCR", "Selecione a Pausa: ", false);
        }

        [WebMethod]
        public static string GetDispositionCode(string dispositionCode)
        {
            return $"Disposition enviado com sucesso, ID: {dispositionCode}";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string ListDispositions(Dictionary<string, string>[] listDispositions)
        {
            try
            {
                var optionsHtml = "<option value=''>Selecione a Disposition</option>";

                foreach (var disposition in listDispositions)
                {
                    string code = disposition["code"];
                    string description = disposition["description"];
                    optionsHtml += $"<option value='{code}'>{description}</option>";
                }

                return optionsHtml;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao processar dispositions", ex);
            }
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static string ReceberObjetoJsonExemplo(Dictionary<string, string>[] listDispositions)
        //{
        //    try
        //    {
        //        var optionsHtml = "<option value=''>Selecione</option>";

        //        foreach (var disposition in listDispositions)
        //        {
        //            string code = disposition["code"];
        //            string description = disposition["description"];
        //            optionsHtml += $"<option value='{code}'>{description}</option>";
        //        }

        //        return optionsHtml;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Erro ao processar dispositions", ex);
        //    }
        //}

        [WebMethod]
        public static string PauseButtonClicked(string reasonId)
        {
            return $"Pausa solicitada com o motivo ID: {reasonId}";
        }

        [WebMethod]
        public static string AgentId(string agentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(agentId))
                {
                    return agentId;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar evento: {ex.Message}");
                throw;
            }
        }

        [WebMethod]
        public static string PassCode(string passCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(passCode))
                {
                    return passCode;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar evento: {ex.Message}");
                throw;
            }
        }

        [WebMethod]
        public static int GetCampaignId(LoginCampaign loginCampaign)
        {
            return loginCampaign.CampaignId;
        }

        [WebMethod]
        public static string GetReceptivaCampaignId(ScreenPop screenPop)
        {
            return $@"{screenPop.campaignId}";
        }

        [WebMethod]
        public static void ScreenPop(ScreenPop screenPop, Funcoes func)
        {
            try
            {
                if (screenPop != null)
                {
                    int callId = screenPop.callId;
                    int campaignId = screenPop.campaignId;
                    string phoneNumber = screenPop.phoneNumber;
                    string campaignData = screenPop.campaignData;
                    string customerId = screenPop.customerId;
                    int agentIdOrigin = screenPop.agentIdOrigin;
                    int campaignIdOrigin = screenPop.campaignIdOrigin;
                    bool readOnly = screenPop.readOnly;
                    string campaignCode = screenPop.campaignCode;
                    string tableName = screenPop.tableName;
                    bool priorityCampaign = screenPop.priorityCampaign;
                    object phoneNumberList = screenPop.phoneNumberList;
                    bool previewCall = screenPop.previewCall;
                    bool automaticPreviewCall = screenPop.automaticPreviewCall;
                    string channel = screenPop.channel;

                    string idDiscador = func.ValorSQL($@"SELECT TOP 1 DC.ID_DISCADOR FROM DISC_CAD DC (NOLOCK) WHERE DC.DESCR = 'OLOS API'");

                    string sql = $@"SELECT TOP 1 1 
                            FROM   POPUPcob (NOLOCK) 
                            WHERE  ID_DISCADOR = '{idDiscador}'
                            AND    ID_USUARIO  = '{func.GetSession("idUsuario", "0")}'
                            AND    ID_LIGACAO  = '{callId}'
                            AND    CPF_CNPJ    = '{customerId}'
                            AND    DDD         = '{phoneNumber.Substring(0, 2)}'
                            AND    TELEFONE    = '{phoneNumber.Substring(2)}'";

                    if (!func.ExisteRegistro(sql))
                    {
                        sql = String.Concat($@"INSERT INTO POPUPcob(ID_DISCADOR,ID_USUARIO,ID_DISC_CAMPANHA,ID_LIGACAO,ID_DISC_STATUS,CPF_CNPJ,DDD,TELEFONE,DATA_INI,DATA_FIM, UNIQUEID, POPUP)
                                            SELECT ID_DISCADOR      = '{idDiscador}',
                                                   ID_USUARIO       = '{func.GetSession("idUsuario", "0")}',
                                                   ID_DISC_CAMPANHA = '{campaignId}',
                                                   ID_LIGACAO       = '{callId}',
                                                   ID_DISC_STATUS   = (SELECT ID_DISC_STATUS FROM DISC_STATUS (NOLOCK) WHERE ID_DISCADOR = {idDiscador} AND DESCR = 'POPUP OLOS API'),
                                                   CPF_CNPJ         = '{customerId}',
                                                   DDD              = '{phoneNumber.Substring(0, 1)}',
                                                   TELEFONE         = '{phoneNumber.Substring(2)}',
                                                   DATA_INI         = GETDATE(),
                                                   DATA_FIM         = NULL,
                                                   UNIQUEID         = NULL,
                                                   POPUP            = '0'");
                        func.Executa_Data(sql);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void btnHangup_ServerClick(object sender, EventArgs e)
        {
            Funcoes func = new Funcoes();

            funcoes.js($@"OlosAgent.hangupRequest({func.GetSession("callId", "0")})");

            updatePanelHangup.Update();
        }
    }
}