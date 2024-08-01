using static OlosAgentSDK.Models.Olos;

using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace OlosAgentSDK.Pages
{
    public partial class SendManualCallRequest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }

        [WebMethod]
        public static void SetManualCallSession(string ddd, string phoneNumber, bool isManualCall)
        {
            HttpContext.Current.Session["DDD"] = ddd;
            HttpContext.Current.Session["TELEFONE"] = phoneNumber;
            HttpContext.Current.Session["IsManualCall"] = isManualCall;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string CheckSendManualCall()
        {
            Funcoes funcoes = new Funcoes();

            try
            {
                var isManualCall = HttpContext.Current.Session["IsManualCall"] != null
                                   && (bool)HttpContext.Current.Session["IsManualCall"];

                // Ligações manuais pelo OLOSweb.
                if (isManualCall)
                {
                    HttpContext.Current.Session["IsManualCall"] = false;
                    return JsonConvert.SerializeObject(new { status = "manual_call", DDD = funcoes.GetSession("DDD", "0"), TELEFONE = funcoes.GetSession("TELEFONE", "0") });
                }
                else
                {
                    // Ligações manuais pelo COBweb.
                    var query = funcoes.ValoresSQL2(
                    $@"
                    SELECT	ID_USUARIO = AA.idUSUARIO,
                            CPF_CNPJ = AA.CPF_CNPJ,
                            DDD = AA.DDD,
                            TELEFONE = AA.TELEFONE,
                            DATA = AA.DATA
                    FROM	POPUPlig_Manual AA (NOLOCK)
                    WHERE	idUSUARIO = '{funcoes.GetSession("idUsuario", "0")}'");

                    if (query != null && query["ID_USUARIO"].Count > 0)
                    {
                        ManualCallData data = new ManualCallData
                        {
                            ID_USUARIO = query["ID_USUARIO"][0].ToString(),
                            CPF_CNPJ = query["CPF_CNPJ"][0].ToString(),
                            DDD = query["DDD"][0].ToString(),
                            TELEFONE = query["TELEFONE"][0].ToString(),
                            DATA = query["DATA"][0].ToString()
                        };

                        funcoes.SetSession("ID_USUARIO", data.ID_USUARIO);
                        funcoes.SetSession("CPF_CNPJ", data.CPF_CNPJ);
                        funcoes.SetSession("DDD", data.DDD);
                        funcoes.SetSession("TELEFONE", data.TELEFONE);

                        string jsonResult = JsonConvert.SerializeObject(data);

                        if (!string.IsNullOrEmpty(data.ID_USUARIO))
                        {
                            funcoes.ValorSQL($@"DELETE  AA 
                                        FROM    POPUPlig_Manual AA (NOLOCK) 
                                        WHERE   AA.idUSUARIO = {data.ID_USUARIO}
                                        AND     AA.CPF_CNPJ = '{data.CPF_CNPJ}'
                                        AND     AA.DDD = '{data.DDD}'
                                        AND     AA.TELEFONE = '{data.TELEFONE}'");
                        }

                        return jsonResult;
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { error = "No data found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string ChangeManualCallState(ChangeManualCallState changeManualCallState)
        {
            Funcoes func = new Funcoes();

            try
            {
                if (changeManualCallState != null)
                {
                    ChangeManualCallState data = new ChangeManualCallState
                    {
                        callId = changeManualCallState.callId,
                        callState = changeManualCallState.callState
                    };

                    func.SetSession("ManualCallId", changeManualCallState.callId);

                    string ID_DISCADOR = func.ValorSQL($@"SELECT TOP 1 DC.ID_DISCADOR FROM DISC_CAD DC (NOLOCK) WHERE DC.DESCR = 'OLOS API'");
                    string ID_USUARIO = func.GetSession("idUsuario", "0").ToString();
                    string ID_LIGACAO = changeManualCallState.callId.ToString();
                    string CPF_CNPJ = func.GetSession("CPF_CNPJ", "0").ToString();
                    string DDD = func.GetSession("DDD", "0").ToString();
                    string TELEFONE = func.GetSession("TELEFONE", "0").ToString();
                    string ID_DISC_CAMPANHA = func.GetSession("CampaignIdAtiva", "0").ToString();

                    func.SetSession("ID_LIGACAO", ID_LIGACAO);

                    string sql = $@"SELECT TOP 1 1 
                                FROM   POPUPcob (NOLOCK) 
                                WHERE  ID_DISCADOR = '{ID_DISCADOR}'
                                AND    ID_USUARIO  = '{ID_USUARIO}'
                                AND    ID_LIGACAO  = '{ID_LIGACAO}'
                                AND    CPF_CNPJ    = '{CPF_CNPJ}'
                                AND    DDD         = '{DDD}'
                                AND    TELEFONE    = '{TELEFONE}'";

                    if (!func.ExisteRegistro(sql))
                    {
                        sql = string.Concat($@"INSERT INTO POPUPcob(ID_DISCADOR,ID_USUARIO,ID_DISC_CAMPANHA,ID_LIGACAO,ID_DISC_STATUS,CPF_CNPJ,DDD,TELEFONE,DATA_INI,DATA_FIM, UNIQUEID, POPUP)
                                            SELECT ID_DISCADOR      = '{ID_DISCADOR}',
                                                   ID_USUARIO       = '{ID_USUARIO}',
                                                   ID_DISC_CAMPANHA = '{ID_DISC_CAMPANHA}',
                                                   ID_LIGACAO       = '{ID_LIGACAO}',
                                                   ID_DISC_STATUS   = (SELECT ID_DISC_STATUS FROM DISC_STATUS (NOLOCK) WHERE ID_DISCADOR = {ID_DISCADOR} AND DESCR = 'POPUP OLOS API'),
                                                   CPF_CNPJ         = '{CPF_CNPJ}',
                                                   DDD              = '{DDD}',
                                                   TELEFONE         = '{TELEFONE}',
                                                   DATA_INI         = GETDATE(),
                                                   DATA_FIM         = NULL,
                                                   UNIQUEID         = NULL,
                                                   POPUP            = '1'");
                        func.Executa_Data(sql);
                    }

                    return JsonConvert.SerializeObject(data);
                }
                else
                {
                    return JsonConvert.SerializeObject(new { error = "No data found" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string CheckManualCallDisposition()
        {
            Funcoes func = new Funcoes();

            try
            {
                if (func.GetSession("ID_LIGACAO", "Não foi retornado nenhum callId") != null)
                {
                    string query = func.ValorSQL($@"
                    SELECT  COD_FIM
                    FROM    POPUPcob (NOLOCK) 
                    WHERE   ID_LIGACAO = {func.GetSession("ID_LIGACAO", "Não foi retornado nenhum callId")}
                    AND     CPF_CNPJ = '{func.GetSession("CPF_CNPJ", "0")}'
                    AND     DDD = '{func.GetSession("DDD", "0")}'
                    AND     TELEFONE = '{func.GetSession("TELEFONE", "0")}'
                    AND     POPUP = 1
                    AND     COD_FIM IS NOT NULL");

                    if (!string.IsNullOrEmpty(query))
                    {
                        func.ValorSQL($@"
                        DELETE FROM POPUPcob
                        WHERE COD_FIM = '{query}'
                        AND ID_LIGACAO = {func.GetSession("ID_LIGACAO", "Não foi retornado nenhum callId")}");
                    }

                    return query;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
