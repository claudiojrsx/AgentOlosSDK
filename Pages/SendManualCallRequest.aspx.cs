using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using static OlosAgentSDK.Models.Olos;

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
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string CheckSendManualCall()
        {
            Funcoes funcoes = new Funcoes();

            try
            {
                var query = funcoes.ValoresSQL2(
                $@"
                SELECT	ID_USUARIO = AA.idUSUARIO,
                        CPF_CNPJ = AA.CPF_CNPJ,
                        DDD = AA.DDD,
                        TELEFONE = AA.TELEFONE,
                        DATA = AA.DATA
                FROM	POPUPlig_Manual AA (NOLOCK)
                WHERE	idUSUARIO = '{funcoes.GetSession("idUsuario", "0")}'");

                if (query != null && query.ContainsKey("ID_USUARIO") && query["ID_USUARIO"].Count > 0)
                {
                    ManualCallData data = new ManualCallData
                    {
                        ID_USUARIO = query["ID_USUARIO"][0]?.ToString(),
                        CPF_CNPJ = query["CPF_CNPJ"][0]?.ToString(),
                        DDD = query["DDD"][0]?.ToString(),
                        TELEFONE = query["TELEFONE"][0]?.ToString(),
                        DATA = query["DATA"][0]?.ToString()
                    };

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
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }
    }
}
