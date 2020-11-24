using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.ApiV3
{
    public class CompanyDirectory : ICompanyDirectory
    {
        private IOAuthHttpClientFactory _httpClientFactory;

        private const string HttpClientConfigurationName = "company-directory-api";
        private HttpClient _client;

        public CompanyDirectory(IOAuthHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Company> Get(string companyId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(CompanyDirectoryUrls.Company(companyId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Company>(content);
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

    internal class CompanyDirectoryUrls
    {
        public static string Root => "/veracity/services/v3/directory/companies";

        public static string Company(string companyId) => $"{Root}/{companyId}";
    }
}
