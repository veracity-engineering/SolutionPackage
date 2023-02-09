using System.Text.Json.Serialization;

namespace DNVGL.Veracity.Services.Api.Models
{
    public class UserReference : Reference
    {
        [JsonPropertyName("activated")]
        public bool IsActivated { get; set; }
    }
}
