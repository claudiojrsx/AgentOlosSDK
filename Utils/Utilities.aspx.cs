using static OlosAgentSDK.Models.Olos;

using System;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web;
using System.Web.Script.Serialization;

namespace OlosAgentSDK.Utils
{
    public partial class Utilities : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
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
                return $"Erro ao processar evento: {ex.Message}";
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
                return $"Erro: {ex.Message}";
            }
        }

        [WebMethod]
        public static string GetActiveConnectionsMethod()
        {
            var activeConnections = Startup.GetActiveConnections();

            int totalConnections = 0;
            foreach (var connection in activeConnections)
            {
                totalConnections += connection.Value;
            }

            return new JavaScriptSerializer().Serialize(new { connections = totalConnections });
        }
    }
}