using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
	public class ServiceDirectory : ApiResourceClient, IServiceDirectory
	{
		public ServiceDirectory(IHttpClientFactory httpClientFactory, ISerializer serializer, OAuthHttpClientOptions option) : base(httpClientFactory, serializer, option)
		{
		}

		/// <summary>
		/// Retrieves an individual service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		public Task<Service> Get(string serviceId) =>
			GetResource<Service>(ServiceDirectoryUrls.Service(serviceId));

		/// <summary>
		/// Retrieves a paginated collection of user references of users subscribed to a service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> ListUsers(string serviceId, int page = 1, int pageSize = 20) =>
			GetResource<IEnumerable<UserReference>>(ServiceDirectoryUrls.ServiceUsers(serviceId, page, pageSize), false);
	}

	internal static class ServiceDirectoryUrls
	{
		public static string Root => "/veracity/services/v3/directory/services";

		public static string Service(string serviceId) => $"{Root}/{serviceId}";

		public static string ServiceUsers(string serviceId, int page, int pageSize) => $"{Service(serviceId)}/users?page={page}&pageSize={pageSize}";
	}
}
