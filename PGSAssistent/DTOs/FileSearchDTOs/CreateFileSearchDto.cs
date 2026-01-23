using System.Text.Json.Serialization;

namespace PGSAssistentAPI.DTOs.FileSearchDTOs
{
    public class CreateFileSearchDto
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = default!;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; init; } = default!;
    }
}
