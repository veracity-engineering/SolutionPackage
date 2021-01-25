using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.ApiV3
{
    public class ServiceDirectory : ApiResourceClient, IServiceDirectory
    {
        private const string HttpClientConfigurationName = "service-directory-api";

        public ServiceDirectory(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer) : base(httpClientFactory, serializer, HttpClientConfigurationName)
        {
        }

        public async Task<Service> Get(string serviceId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ServiceDirectoryUrls.Service(serviceId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Service>(content);
        }

        public async Task<IEnumerable<UserReference>> ListUsers(string serviceId, int page = 1, int pageSize = 20)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ServiceDirectoryUrls.ServiceUsers(serviceId, page, pageSize));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<UserReference>>(content);
        }
    }

    internal class ServiceDirectoryUrls
    {
        public static string Root => "/veracity/services/v3/directory/services";

        public static string Service(string serviceId) => $"{Root}/{serviceId}";

        public static string ServiceUsers(string serviceId, int page, int pageSize) => $"{Service(serviceId)}/users?page={page}&pageSize={pageSize}";
    }
}
