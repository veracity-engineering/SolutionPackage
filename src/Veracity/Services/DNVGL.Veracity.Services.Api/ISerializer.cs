using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api
{
	public enum DataFormat
	{
		Json,
        Xml
	}

    public interface ISerializer
    {
	    DataFormat DataFormat { get; }

        string Serialize<T>(T value);

        Task SerializeAsync<T>(T value, Stream stream, CancellationToken cancellationToken = default);

        T? Deserialize<T>(string strValue);

        Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default);
    }
}
