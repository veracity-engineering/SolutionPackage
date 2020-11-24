using Newtonsoft.Json;

namespace DNVGL.Veracity.Services.Api.Models
{
    public class UserReference : Reference
    {
        [JsonProperty("activated")]
        public bool IsActivated { get; set; }
    }
}
