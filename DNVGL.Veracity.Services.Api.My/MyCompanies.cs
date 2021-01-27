using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyCompanies : ApiResourceClient, IMyCompanies
    {
        public MyCompanies(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<IEnumerable<CompanyReference>> List()
        {
            var response = await GetOrCreateHttpClient().GetAsync(MyCompaniesUrls.Root);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<CompanyReference>>(content);
        }
    }

    internal class MyCompaniesUrls
    {
        public static string Root => "/veracity/services/v3/my/companies";
    }
}
