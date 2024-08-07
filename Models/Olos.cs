﻿namespace OlosAgentSDK.Models
{
    public class Olos
    {
        public class LoginCampaign
        {
            public int AgentId { get; set; }
            public int CampaignId { get; set; }
            public string CampaignName { get; set; }
            public string CampaignCode { get; set; }
        }

        public class ScreenPop
        {
            public int callId { get; set; }
            public int campaignId { get; set; }
            public string phoneNumber { get; set; }
            public string campaignData { get; set; }
            public string customerId { get; set; }
            public int agentIdOrigin { get; set; }
            public int campaignIdOrigin { get; set; }
            public bool readOnly { get; set; }
            public string campaignCode { get; set; }
            public string tableName { get; set; }
            public bool priorityCampaign { get; set; }
            public object phoneNumberList { get; set; }
            public bool previewCall { get; set; }
            public bool automaticPreviewCall { get; set; }
            public string channel { get; set; }
        }

        public class ChangeStatus
        {
            public int agentId { get; set; }
            public string agentStatusId { get; set; }
            public int reasonId { get; set; }
            public string reasonDescription { get; set; }
        }

        public class ManualCallData
        {
            public string ID_USUARIO { get; set; }
            public string CPF_CNPJ { get; set; }
            public string DDD { get; set; }
            public string TELEFONE { get; set; }
            public string DATA { get; set; }
        }

        public class ChangeManualCallState
        {
            public int callId { get; set; }
            public string callState { get; set; }
        }
    }
}