using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api
{
    public class JsonSerializer : ISerializer
    {
	    private readonly Newtonsoft.Json.JsonSerializer _jsonSerializer;

	    public DataFormat DataFormat => DataFormat.Json;

		public JsonSerializer(Newtonsoft.Json.JsonSerializer jsonSerializer = null)
		{
			_jsonSerializer = jsonSerializer ?? Newtonsoft.Json.JsonSerializer.Create();
		}

		public T Deserialize<T>(string value)
		{
			using (var reader = new StringReader(value))
			using (var jsonReader = new JsonTextReader(reader))
				return _jsonSerializer.Deserialize<T>(jsonReader);
		}

		public T Deserialize<T>(Stream stream)
		{
			using (var reader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(reader))
				return _jsonSerializer.Deserialize<T>(jsonReader);
		}

		public string Serialize<T>(T value)
		{
			var sb = new StringBuilder(1000);

			using (var writer = new StringWriter(sb))
			using (var jsonWriter = new JsonTextWriter(writer))
				_jsonSerializer.Serialize(jsonWriter, value, typeof(T));

			return sb.ToString();
		}

		public void Serialize<T>(T value, Stream stream)
		{
			using (var writer = new StreamWriter(stream))
			using (var jsonWriter = new JsonTextWriter(writer))
				_jsonSerializer.Serialize(jsonWriter, value, typeof(T));
		}
	}
}
