using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisAdministrators : ApiResourceClient, IThisAdministrators
    {
        public ThisAdministrators(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<Administrator> Get(string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisAdministratorsUrls.Administrator(userId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Administrator>(content);
        }

        public async Task<IEnumerable<AdministratorReference>> List(int page, int pageSize)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisAdministratorsUrls.List(page, pageSize));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<AdministratorReference>>(content);
        }
    }

    internal class ThisAdministratorsUrls
    {
        public static string Root => "/veracity/services/v3/this/administrators";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string Administrator(string userId) => $"{Root}/{userId}";
    }
}
