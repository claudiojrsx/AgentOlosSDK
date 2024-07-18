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

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            QueryStrings.QueryStringParameters queryStringParameters = new QueryStrings.QueryStringParameters
            {
                agentLogin = funcoes.COBCRPTO(Request.QueryString["A"]),
                agentPassword = funcoes.COBCRPTO(Request.QueryString["B"]),
                idUsuario = funcoes.COBCRPTO(Request.QueryString["C"]),
                dbServerIp = funcoes.COBCRPTO(Request.QueryString["D"]),
                dbName = funcoes.COBCRPTO(Request.QueryString["E"]),
                dbLogin = funcoes.COBCRPTO(Request.QueryString["F"]),
                dbPassword = funcoes.COBCRPTO(Request.QueryString["G"]),
                dbPort = funcoes.COBCRPTO(Request.QueryString["H"])
            };

            funcoes.SetSession("idUsuario", Request.QueryString["C"]);

            funcoes.Connection(
                queryStringParameters.dbServerIp,
                queryStringParameters.dbName,
                queryStringParameters.dbLogin,
                queryStringParameters.dbPassword,
                null,
                null,
                queryStringParameters.dbPort);

            if (!string.IsNullOrEmpty(queryStringParameters.agentLogin) && !string.IsNullOrEmpty(queryStringParameters.agentPassword))
            {
                funcoes.js($"OlosAgent.authenticatedOlos('{queryStringParameters.agentLogin}', '{queryStringParameters.agentPassword}');", this);
            }
        }

        [WebMethod]
        public static string GetDispositionCode(string dispositionCode)
        {
            return $"Disposition enviado com sucesso, ID: {dispositionCode}";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetListReasons(Dictionary<string, string>[] reasons)
        {
            try
            {
                var optionsHtml = "<option value=''>Selecione a Pausa</option>";

                foreach (var reason in reasons)
                {
                    string reasonCode = reason["reasonCode"];
                    string description = reason["description"];
                    optionsHtml += $"<option value='{reasonCode}'>{description}</option>";
                }

                return optionsHtml;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao processar ListReasons", ex);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetListDispositions(Dictionary<string, string>[] dispositions)
        {
            try
            {
                var optionsHtml = "<option value=''>Selecione a Disposition</option>";

                foreach (var disposition in dispositions)
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

        [WebMethod]
        public static string GetReasonId(string reasonId)
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
        public static void ScreenPop(ScreenPop screenPop)
        {
            Funcoes func = new Funcoes();

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
                                                   DDD              = '{phoneNumber.Substring(0, 2)}',
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
    }
}