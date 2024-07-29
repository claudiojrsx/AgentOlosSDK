using static OlosAgentSDK.Models.Olos;

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

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
                    funcoes.js($"OlosAgent.authenticatedOlos('{agentLogin}', '{agentPassword}');", this);
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Error: {ex.Message}");
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
                    string customerId = screenPop.customerId;

                    func.SetSession("callId", callId);
                    func.SetSession("customerId", customerId);
                    func.SetSession("phoneNumberDDD", phoneNumber.Substring(0, 2));
                    func.SetSession("phoneNumber", phoneNumber.Substring(2));

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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string CheckScreenPop()
        {
            Funcoes func = new Funcoes();
            try
            {
                string query = func.ValorSQL($@"
                SELECT  COD_FIM
                FROM    POPUPcob (NOLOCK) 
                WHERE   ID_LIGACAO = '{func.GetSession("callId", "0")}'
                AND     CPF_CNPJ = '{func.GetSession("customerId", "0")}'
                AND     DDD = '{func.GetSession("phoneNumberDDD", "0")}'
                AND     TELEFONE = '{func.GetSession("phoneNumber", "0")}'
                AND     COD_FIM IS NOT NULL");

                if (!string.IsNullOrEmpty(query))
                {
                    func.ValorSQL($@"
                    DELETE FROM POPUPcob
                    WHERE COD_FIM = '{query}'
                    AND ID_LIGACAO = '{func.GetSession("callId", "0")}'");
                }

                return query;
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public static string ChangeStatus(ChangeStatus changestatus)
        {
            try
            {
                int agentId = changestatus.agentId;
                string agentStatusId = changestatus.agentStatusId;
                int reasonId = changestatus.reasonId;
                string reasonDescription = changestatus.reasonDescription;

                return agentStatusId;
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }
    }
}