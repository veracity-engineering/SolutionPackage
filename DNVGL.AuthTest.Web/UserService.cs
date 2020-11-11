using DNVGL.OAuth.Api.HttpClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.AuthTest.Web
{
    public class UserService
    {
        private readonly IOAuthHttpClientFactory _httpClientFactory;

        private const string V1Path = "/internal/test/identity/v1";

        public UserService(IOAuthHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Get signed in user from identity.
        /// </summary>
        /// <returns>JSON string from response body.</returns>
        public async Task<string> GetUser()
        {
            using (var client = BuildHttpClient("identity-api-user"))
            {
                var response = await client.GetAsync($"{V1Path}/users/me");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Get user by specified id.
        /// </summary>
        /// <returns>JSON string from response body.</returns>
        public async Task<string> GetUserById(string id)
        {
            using (var client = BuildHttpClient("identity-api-client"))
            {
                var response = await client.GetAsync($"{V1Path}/users/{id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Get user by specified email address.
        /// </summary>
        /// <returns>JSON string from response body.</returns>
        public async Task<string> GetUserByEmail(string email)
        {
            using (var client = BuildHttpClient("identity-api-client"))
            {
                var response = await client.GetAsync($"{V1Path}/users/.email?q={HttpUtility.UrlEncode(email)}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        private HttpClient BuildHttpClient(string name)
        {
            var client = _httpClientFactory.Create(name);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }
    }
}
