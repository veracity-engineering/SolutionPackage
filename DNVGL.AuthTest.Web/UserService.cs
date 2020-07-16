using DNVGL.OAuth.Api.HttpClient;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<object> GetUser()
        {
            using (var client = BuildHttpClient())
            {
                var response = await client.GetAsync($"{V1Path}/users/me");
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            }
        }

        private HttpClient BuildHttpClient()
        {
            var client = _httpClientFactory.Create("identity-api");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }
    }
}
