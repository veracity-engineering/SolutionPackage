using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
	public class MyServices : ApiResourceClient, IMyServices
	{
		public MyServices(IHttpClientFactory httpClientFactory, ISerializer serializer, OAuthHttpClientOptions option) : base(httpClientFactory, serializer, option)
		{
		}

		/// <summary>
		/// Retrieves a collection of service references for services the authenticated user is subscribed to.
		/// </summary>
		/// <returns></returns>
		public Task<IEnumerable<MyServiceReference>> List() =>
			GetResource<IEnumerable<MyServiceReference>>(MyServicesUrls.Root, false);
	}

	internal static class MyServicesUrls
	{
		public static string Root => "/veracity/services/v3/my/services";
	}
}
