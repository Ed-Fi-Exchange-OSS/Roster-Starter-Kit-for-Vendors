using Newtonsoft.Json;

namespace EdFi.Roster.Models
{
    public class BearerTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
