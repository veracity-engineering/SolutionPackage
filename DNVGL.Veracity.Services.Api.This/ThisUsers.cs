using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisUsers : ApiResourceClient, IThisUsers
    {
        public ThisUsers(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<CreateUserReference> Create(CreateUserOptions options)
        {
            var response = await GetOrCreateHttpClient().PostAsync(ThisUsersUrls.UserRoot, new StringContent(Serialize(options)));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<CreateUserReference>(content);
        }

        public async Task<IEnumerable<CreateUserReference>> Create(params CreateUserOptions[] options)
        {
            var response = await GetOrCreateHttpClient().PostAsync(ThisUsersUrls.UsersRoot, new StringContent(Serialize(options)));
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

    internal static class ThisUsersUrls
    {
        public static string UsersRoot => "/veracity/services/v3/this/users";

        public static string UserRoot => "/veracity/services/v3/this/user";

        public static string Resolve(string email) => $"{UserRoot}/resolve({HttpUtility.UrlEncode(email)})";
    }
}
