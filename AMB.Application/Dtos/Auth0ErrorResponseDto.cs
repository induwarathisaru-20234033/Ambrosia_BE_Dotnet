using System.Text.Json.Serialization;

namespace AMB.Application.Dtos
{
    public class Auth0ErrorResponseDto
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("statusCode")]
        public int? StatusCode { get; set; }
    }
}
