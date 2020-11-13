using DNVGL.OAuth.Api.HttpClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.AuthTest.Web
{
    public interface IUserService
    {
        /// <summary>
        /// Get signed in user.
        /// </summary>
        /// <returns>JSON string from response body.</returns>
        Task<string> GetUser();

        /// <summary>
        /// Get user by specified id.
        /// </summary>
        /// <returns>JSON string from response body.</returns>
        Task<string> GetUserById(string id);

        /// <summary>
        /// Get user by specified email address.
        /// </summary>
        /// <returns>JSON string from response body.</returns>
        Task<string> GetUserByEmail(string email);
    }

    public class UserService : IUserService
    {
        private readonly IOAuthHttpClientFactory _httpClientFactory;
        private IDictionary<string, HttpClient> _httpClients;

        private const string V1Path = "/internal/test/identity/v1";
        private const string UserCredentialsClientName = "identity-api-user";
        private const string ClientCredentialsClientName = "identity-api-client";

        private HttpClient UserCredentialsHttpClient => GetOrCreateHttpClient(UserCredentialsClientName);
        private HttpClient ClientCredentialsHttpClient => GetOrCreateHttpClient(ClientCredentialsClientName);

        public UserService(IOAuthHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClients = new Dictionary<string, HttpClient>();
        }

        public async Task<string> GetUser()
        {
            var response = await UserCredentialsHttpClient.GetAsync($"{V1Path}/users/me");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetUserById(string id)
        {
            var response = await ClientCredentialsHttpClient.GetAsync($"{V1Path}/users/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetUserByEmail(string email)
        {
            var response = await ClientCredentialsHttpClient.GetAsync($"{V1Path}/users/.email?q={HttpUtility.UrlEncode(email)}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private HttpClient GetOrCreateHttpClient(string name)
        {
            if (!_httpClients.ContainsKey(name))
            {
                var client = _httpClientFactory.Create(name);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                _httpClients.Add(name, client);
            }
            return _httpClients[name];
        }
    }
}
