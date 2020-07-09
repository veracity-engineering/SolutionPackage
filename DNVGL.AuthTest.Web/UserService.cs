using DNVGL.OAuth.UserCredentials;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.AuthTest.Web
{
    public class UserService
    {
        private IOAuthHttpClientFactory _httpClientFactory;

        public UserService(IOAuthHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<object> GetUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/users/me");
            using (var client = _httpClientFactory.Create("identity-api"))
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
