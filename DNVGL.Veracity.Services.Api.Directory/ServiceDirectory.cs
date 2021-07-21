using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
	public class ServiceDirectory : ApiResourceClient, IServiceDirectory
	{
		public ServiceDirectory(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
		{
		}

		public Task<Service> Get(string serviceId) =>
			GetResult<Service>(ServiceDirectoryUrls.Service(serviceId));

		public Task<IEnumerable<UserReference>> ListUsers(string serviceId, int page = 1, int pageSize = 20) =>
			GetResult<IEnumerable<UserReference>>(ServiceDirectoryUrls.ServiceUsers(serviceId, page, pageSize), false);
	}

	internal static class ServiceDirectoryUrls
	{
		public static string Root => "/veracity/services/v3/directory/services";

		public static string Service(string serviceId) => $"{Root}/{serviceId}";

		public static string ServiceUsers(string serviceId, int page, int pageSize) => $"{Service(serviceId)}/users?page={page}&pageSize={pageSize}";
	}
}
