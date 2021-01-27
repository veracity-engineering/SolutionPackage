using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.Veracity.Services.Api.This.ApiV3
{
    public class ThisUsers : ApiResourceClient, IThisUsers
    {
        public ThisUsers(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<CreateUserReference> Create(CreateUserOptions options)
        {
            var response = await GetOrCreateHttpClient().PostAsync(ThisUsersUrls.Root, new StringContent(Serialize(options)));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<CreateUserReference>(content);
        }

        public async Task<IEnumerable<CreateUserReference>> Create(params CreateUserOptions[] options)
        {
            var response = await GetOrCreateHttpClient().PostAsync(ThisUsersUrls.Root, new StringContent(Serialize(options)));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<CreateUserReference>>(content);
        }

        public async Task<IEnumerable<UserReference>> Resolve(string email)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisUsersUrls.Resolve(email));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<CreateUserReference>>(content);
        }
    }

    internal class ThisUsersUrls
    {
        public static string Root => "/veracity/services/v3/this/users";

        public static string Resolve(string email) => $"{Root}/resolve({HttpUtility.UrlEncode(email)})";
    }
}
