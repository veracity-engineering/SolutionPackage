using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JSerializer= System.Text.Json.JsonSerializer;

namespace DNVGL.Veracity.Services.Api
{
    public class JsonSerializer : ISerializer
    {
	    private readonly JsonSerializerOptions? _jsonSerializerOptions;

	    public DataFormat DataFormat => DataFormat.Json;

		public JsonSerializer(JsonSerializerOptions? jsonSerializerOptions = null)
		{
			_jsonSerializerOptions = jsonSerializerOptions;
		}

		public T? Deserialize<T>(string strValue)
		{
			return JSerializer.Deserialize<T>(strValue, _jsonSerializerOptions);
		}

		public Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
		{
			return JSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions, cancellationToken).AsTask();
		}

		public string Serialize<T>(T value)
		{
			return JSerializer.Serialize(value, _jsonSerializerOptions);
		}

		public Task SerializeAsync<T>(T value, Stream stream, CancellationToken cancellationToken)
		{
			return JSerializer.SerializeAsync(stream, value, _jsonSerializerOptions, cancellationToken);
		}
	}
}
