using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.ApiV3
{
    public class MyProfile : IMyProfile
    {
        private IOAuthHttpClientFactory _httpClientFactory;

        private const string HttpClientConfigurationName = "profile-my-api";
        private HttpClient _client;

        public MyProfile(IOAuthHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Profile> Get()
        {
            var response = await GetOrCreateHttpClient().GetAsync(ProfileMyUrls.Root);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Profile>(content);
        }

        private HttpClient GetOrCreateHttpClient()
        {
            if (_client == null)
            {
                _client = _httpClientFactory.Create(HttpClientConfigurationName);
                _client.DefaultRequestHeaders.Add("Accept", "application/json");
            }
            return _client;
        }

        private T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value);
    }

    internal class ProfileMyUrls
    {
        public static string Root => "/veracity/services/v3/my/profile";
    }
}
