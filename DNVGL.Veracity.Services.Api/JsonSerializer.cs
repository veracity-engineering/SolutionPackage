using Newtonsoft.Json;

namespace DNVGL.Veracity.Services.Api
{
    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value);

        public string Serialize<T>(T value) => JsonConvert.SerializeObject(value);
    }
}
