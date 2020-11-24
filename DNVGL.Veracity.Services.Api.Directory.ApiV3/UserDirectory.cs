using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.ApiV3
{
    public class UserDirectory : ApiResourceClient, IUserDirectory
    {
        private const string HttpClientConfigurationName = "user-directory-api";

        public UserDirectory(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer) : base(httpClientFactory, serializer, HttpClientConfigurationName)
        {
        }

        public async Task<User> Get(string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.User(userId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<User>(content);
        }

        public async Task Delete(string userId)
        {
            var response = await GetOrCreateHttpClient().DeleteAsync(UserDirectoryUrls.User(userId));
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<User>> ListByEmail(string email)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersByEmail(email));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<User>>(content);
        }

        public async Task<IEnumerable<CompanyReference>> ListCompanies(string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersCompanies(userId));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<CompanyReference>>(content);
        }

        public async Task<IEnumerable<ServiceReference>> ListServices(string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersServices(userId));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<ServiceReference>>(content);
        }

        public async Task<Subscription> GetSubscription(string userId, string serviceId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersServiceSubscription(userId, serviceId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Subscription>(content);
        }
    }

    internal class UserDirectoryUrls
    {
        public static string Root => "/veracity/services/v3/directory/users";

        public static string User(string userId) => $"{Root}/{userId}";

        public static string UsersByEmail(string email) => $"{Root}/by/.email={email}";

        public static string UsersCompanies(string userId) => $"{User(userId)}/companies";

        public static string UsersServices(string userId) => $"{User(userId)}/services";

        public static string UsersServiceSubscription(string userId, string serviceId) => $"{UsersServices(userId)}/{serviceId}";
    }
}
