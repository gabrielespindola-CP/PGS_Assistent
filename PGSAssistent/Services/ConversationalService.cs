using Google.GenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PGSAssistent.Configuration;
using PGSAssistentAPI.DTOs.ConversationalDTOs;
using System.Threading.Tasks;

namespace PGSAssistent.Services
{
    public class ConversationalService
    {
        private readonly GeminiSettings _geminiSettings;
        private readonly ILogger<ConversationalService> _logger;

        public ConversationalService(IOptions<GeminiSettings> options, ILogger<ConversationalService> logger) 
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
                    contents: "Responda apenas Gemini está pronto para uso"
                    );

                var text = response.Candidates[0].Content.Parts[0].Text;
                _logger.LogInformation("Gemini está pronto para uso");
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao inicialicar Gemini.");
                return ("Falha ao inicialicar Gemini.");
            }
        }

        internal async Task<ResponseDto> ExecutePromptAsync(string prompt, CancellationToken ct)
        {
            ResponseDto response = new ResponseDto();
            var apiKey = _geminiSettings.ApiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogInformation("Não foi encontrado APIKey do Gemini");
                response.Response = "Não foi encontrado APIKey do Gemini";
                return (response);
            }

            try
            {
                var storeName = _geminiSettings.FileSearchName;
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_geminiSettings.ApiKey}";
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao inicialicar Gemini.");
                response.Response = "Não foi encontrado APIKey do Gemini";
                return (response);
            }
        }
    }
}
