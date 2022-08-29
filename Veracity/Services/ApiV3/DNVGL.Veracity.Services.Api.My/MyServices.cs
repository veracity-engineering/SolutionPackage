using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
	public class MyServices : ApiClientBase, IMyServices
	{		
		public MyServices(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
			: base(optionsList, httpClientFactory, serializer)
		{

		}

		/// <summary>
		/// Retrieves a collection of service references for services the authenticated user is subscribed to.
		/// </summary>
		/// <returns></returns>
		public Task<IEnumerable<MyServiceReference>> List() =>
			base.GetClient().GetResource<IEnumerable<MyServiceReference>>(MyServicesUrls.Root, false);
	}

	internal static class MyServicesUrls
	{
		public static string Root => "/veracity/services/v3/my/services";
	}
}
