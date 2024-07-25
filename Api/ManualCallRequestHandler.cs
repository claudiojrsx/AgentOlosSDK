using System;

namespace OlosAgentSDK.Api
{
    public class ManualCallRequestHandler
    {
        public string AgentId { get; private set; }
        public string Ddd { get; private set; }
        public string PhoneNumber { get; private set; }
        public string CampaignId { get; private set; }
        public string Mensagem { get; private set; }

        public ManualCallRequestHandler(string agentId, string ddd, string phoneNumber, string campaignId)
        {
            AgentId = agentId;
            Ddd = ddd;
            PhoneNumber = phoneNumber;
            CampaignId = campaignId;
            ProcessParameters();
        }

        private void ProcessParameters()
        {
            if (!string.IsNullOrEmpty(AgentId) && !string.IsNullOrEmpty(Ddd) &&
                !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(CampaignId))
            {
                Mensagem = "Parâmetros recebidos e processados com sucesso!";
            }
            else
            {
                throw new ArgumentException("Parâmetros ausentes ou inválidos.");
            }
        }

        public string ToJson()
        {
            var resultado = new
            {
                AgentId,
                Ddd,
                PhoneNumber,
                CampaignId,
                Mensagem
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(resultado);
        }
    }
}