using System;

namespace OlosAgentSDK
{
    public class ParameterHandler
    {
        public string Param1 { get; private set; }
        public string Param2 { get; private set; }
        public string Mensagem { get; private set; }

        public ParameterHandler(string param1, string param2)
        {
            Param1 = param1;
            Param2 = param2;
            ProcessParameters();
        }

        private void ProcessParameters()
        {
            if (!string.IsNullOrEmpty(Param1) && !string.IsNullOrEmpty(Param2))
            {
                Mensagem = "Parâmetros recebidos com sucesso!";
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
                Param1,
                Param2,
                Mensagem
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(resultado);
        }
    }
}
