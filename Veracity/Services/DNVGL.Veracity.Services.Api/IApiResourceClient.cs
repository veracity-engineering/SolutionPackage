using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api
{
	public interface IApiResourceClient
	{			
		ISerializer Serializer { get; }
		Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
	};
}
