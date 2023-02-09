using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api
{
	public interface IApiClient
	{			
		ISerializer Serializer { get; }
		Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
	};
}
