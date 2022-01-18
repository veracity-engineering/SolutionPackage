using System.IO;
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

        void Serialize<T>(T value, Stream stream);

        T Deserialize<T>(string value);

        T Deserialize<T>(Stream stream);
    }
}
