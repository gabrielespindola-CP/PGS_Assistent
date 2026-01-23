using Google.GenAI;
using Microsoft.Extensions.Options;
using PGSAssistent.Configuration;
using PGSAssistent.Services;
using PGSAssistentAPI.DTOs.FileSearchDTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PGSAssistentAPI.Services
{
    public class FileSearchService
    {
        private readonly GeminiSettings _geminiSettings;
        private readonly ILogger<ConversationalService> _logger;
        private readonly HttpClient _http;

        public FileSearchService(IOptions<GeminiSettings> options, ILogger<ConversationalService> logger, HttpClient http)
        {
            _geminiSettings = options.Value;
            _logger = logger;
            _http = http;
        }

        public async Task<CreateFileSearchDto> Create()
        {
            var url = $"v1beta/fileSearchStores?key={_geminiSettings.ApiKey}";

            var payload = new
            {
                display_name = "FileSearchPGSAssistent"
            };

            using var resp = await _http.PostAsJsonAsync(url, payload);

            if (resp.IsSuccessStatusCode)
            {
                var store = await resp.Content.ReadFromJsonAsync<CreateFileSearchDto>();
                return store ?? throw new InvalidOperationException("Resposta inválida.");
            }

            var errorBody = await resp.Content.ReadAsStringAsync();
            _logger.LogError("Erro ao criar FileSearchStore: {Error}", errorBody);

            throw new Exception($"Falha ao criar FileSearchStore: {resp.StatusCode}");
        }

        public async Task UploadAllPdfsFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                _logger.LogError("O diretório {Path} não foi encontrado.", folderPath);
                return;
            }

            var pdfFiles = Directory.EnumerateFiles(folderPath, "*.pdf");

            foreach (var filePath in pdfFiles)
            {
                try
                {
                    _logger.LogInformation("Iniciando upload de: {FileName}", Path.GetFileName(filePath));
                    await UploadDirectToStore(filePath);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao subir o arquivo {File}", filePath);
                }
            }
            _logger.LogInformation("Processo de upload da pasta concluído.");
        }

        private async Task UploadDirectToStore(string filePath)
        {
            var storeName = _geminiSettings.FileSearchName;
            var url = $"https://generativelanguage.googleapis.com/upload/v1beta/{storeName}:uploadToFileSearchStore?key={_geminiSettings.ApiKey}";

            using var form = new MultipartFormDataContent();

            var metadataObj = new
            {
                displayName = Path.GetFileName(filePath),
            };

            var jsonPayload = JsonSerializer.Serialize(metadataObj);
            var metadataContent = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            metadataContent.Headers.Remove("Content-Disposition");
            metadataContent.Headers.TryAddWithoutValidation("Content-Disposition", "form-data; name=\"metadata\"");
            form.Add(metadataContent);

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            fileContent.Headers.Remove("Content-Disposition");
            fileContent.Headers.TryAddWithoutValidation("Content-Disposition", $"form-data; name=\"file\"; filename=\"{Path.GetFileName(filePath)}\"");
            form.Add(fileContent);

            var response = await _http.PostAsync(url, form);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                _logger.LogError("Detalhe do Erro 400: {Detail}", errorDetail);
                throw new Exception($"Erro no Google Gemini ({response.StatusCode}): {errorDetail}");
            }

            var operationResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Sucesso! Operação: {Op}", operationResponse);
        }
    }
}

