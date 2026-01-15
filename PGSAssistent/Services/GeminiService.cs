using Google.GenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PGSAssistent.Configuration;
using System.Threading.Tasks;

namespace PGSAssistent.Services
{
    public class GeminiService
    {
        private readonly GeminiSettings _geminiSettings;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(IOptions<GeminiSettings> options, ILogger<GeminiService> logger) 
        {
            _geminiSettings = options.Value;
            _logger = logger;
        }

        public async Task<string> ExececutePrompt()
        {
            var apiKey = _geminiSettings.ApiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogInformation("Não foi encontrado APIKey do Gemini");
                return ("Não foi encontrado APIKey do Gemini");
            }

            try
            {
                var client = new Client(apiKey: apiKey);
                var response = await client.Models.GenerateContentAsync(
                    model: _geminiSettings.Model,
                    contents: "Responda apenas Ok"
                    );

                var text = response.Candidates[0].Content.Parts[0].Text;
                _logger.LogInformation($"Gemini pronto. Ele disse {text}");
                return text;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Falha ao inicialicar Gemini.");
                return ("Falha ao inicialicar Gemini.");
            }
        }
    }
}
