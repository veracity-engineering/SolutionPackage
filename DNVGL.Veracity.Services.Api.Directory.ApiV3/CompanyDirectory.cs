using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.ApiV3
{
    public class CompanyDirectory : ApiResourceClient, ICompanyDirectory
    {
        private const string HttpClientConfigurationName = "company-directory-api";

        public CompanyDirectory(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer) : base(httpClientFactory, serializer, HttpClientConfigurationName)
        {
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
    }

    internal class CompanyDirectoryUrls
    {
        public static string Root => "/veracity/services/v3/directory/companies";

        public static string Company(string companyId) => $"{Root}/{companyId}";
    }
}
