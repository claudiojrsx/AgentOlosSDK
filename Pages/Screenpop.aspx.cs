using static OlosAgentSDK.Models.Olos;

using System;
using System.Web.Services;
using System.Web;
using System.Web.Script.Services;

namespace OlosAgentSDK.Pages
{
    public partial class Screenpop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
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
    }
}