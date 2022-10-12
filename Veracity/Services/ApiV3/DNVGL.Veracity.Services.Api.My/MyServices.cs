using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyServices :  IMyServices
	{
        private readonly ApiClientFactory _apiClientFactory;
        public MyServices(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        /// <summary>
        /// Retrieves a collection of service references for services the authenticated user is subscribed to.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<MyServiceReference>> List() =>
            _apiClientFactory.GetClient().GetResource<IEnumerable<MyServiceReference>>(MyServicesUrls.Root, false);
	}

	internal static class MyServicesUrls
	{
		public static string Root => "/veracity/services/v3/my/services";
	}
}
