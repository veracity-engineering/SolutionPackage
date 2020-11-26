using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.ApiV3
{
    public class MyCompanies : ApiResourceClient, IMyCompanies
    {
        private const string HttpClientConfigurationName = "companies-my-api";

        public MyCompanies(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer) : base(httpClientFactory, serializer, HttpClientConfigurationName)
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
