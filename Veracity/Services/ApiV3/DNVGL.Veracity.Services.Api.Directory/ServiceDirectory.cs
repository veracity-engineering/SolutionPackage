using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
	public class ServiceDirectory : IServiceDirectory
	{
		private readonly ApiClientFactory _apiClientFactory;
        public ServiceDirectory(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        /// <summary>
        /// Retrieves an individual service.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public Task<Service> Get(string serviceId) =>
            _apiClientFactory.GetClient().GetResource<Service>(ServiceDirectoryUrls.Service(serviceId));

		/// <summary>
		/// Retrieves a paginated collection of user references of users subscribed to a service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> ListUsers(string serviceId, int page = 1, int pageSize = 20) =>
            _apiClientFactory.GetClient().GetResource<IEnumerable<UserReference>>(ServiceDirectoryUrls.ServiceUsers(serviceId, page, pageSize), false);

		public Task<IEnumerable<Subscription>> GetServiceSubscriptions(string serviceId, string filter, string pageNo) =>
            _apiClientFactory.GetClient().GetResource<IEnumerable<Subscription>>(ServiceDirectoryUrls.GetServiceSubscriptions(serviceId, filter, pageNo), false);

		public Task<bool> IsAdmin(string serviceId, string userId)
			=> _apiClientFactory.GetClient().GetResource<bool>(ServiceDirectoryUrls.IsAdmin(serviceId, userId), false);
			
	}

	internal static class ServiceDirectoryUrls
	{
		public static string Root => "/veracity/services/v3/directory/services";

		public static string Service(string serviceId) => $"{Root}/{serviceId}";

		public static string ServiceUsers(string serviceId, int page, int pageSize) => $"{Service(serviceId)}/users?page={page}&pageSize={pageSize}";


		public static string GetServiceSubscriptions(string serviceId,string filter,string pageNo) => $"{Root}/{serviceId}/subscribers?filter={filter}&pageNo={pageNo}";

		public static string IsAdmin(string serviceId,string userId) => $"{Root}/{serviceId}/administrators/{userId}";

	}
}
