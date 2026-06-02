
using Google.GenAI;

namespace VHBurguer.Applications.ContentSafety
{
    public class ContentSafetyService : IContentSafetyRepository
    {
        private readonly string _apiKey;

        public ContentSafetyService(IConfiguration config)
        {


            _apiKey = config["Gemini:ApiKey"] ??
                Environment.GetEnvironmentVariable("GEMINI_API_KEY") ??
                throw new Exception("API KEY nao configurada");
        }

        public async Task<(bool aprovado, string msg)> ValidarConteudo(string texto)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return (false, "API KEY nao configurada");
            }

            try
            {
                Client client = new Client(apiKey: _apiKey);

                string prompt = $@"Você é um moderador de conteúdo extremamente rigoroso para uma plataforma pública.

                    Analise o TEXTO abaixo considerando as regras:

                    - NÃO é permitido:
                      - palavrões, xingamentos ou linguagem vulgar (ex: ""caralho"", ""porra"", ""merda"", etc.)
                      - conteúdo ofensivo, agressivo ou desrespeitoso
                      - conteúdo com duplo sentido ou conotação sexual
                      - qualquer linguagem inadequada para ambiente profissional ou educacional
                      - conteúdo ilegal (drogas, armas, etc.)

                    - Mesmo que esteja em tom informal ou ""brincadeira"", ainda deve ser considerado INSEGURO.

                    - Seja extremamente conservador: na dúvida, classifique como INSEGURO.

                    Responda APENAS com:

                    SEGURO ou INSEGURO: [breve motivo em português]

                    TEXTO:{texto}";

                //Emvoa p textp para analise da IA

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash-lite",
                    contents: prompt
                    );

                string result = response.Text?.Trim() ?? "";


                if (result.StartsWith("INSEGURO"))
                {
                    return (false, result);
                }

                return (true, "Texto confirmado!");

            }
            catch (Exception ex)
            {
                return(false, "Erro na confirmação da mensagem: "+ex.Message);
            }
        }
    }
}
