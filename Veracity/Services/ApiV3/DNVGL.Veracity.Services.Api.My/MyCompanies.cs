using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyCompanies : ApiResourceClient, IMyCompanies
    {
        public MyCompanies(IHttpClientFactory httpClientFactory, ISerializer serializer, OAuthHttpClientOptions option) : base(httpClientFactory, serializer, option)
        {
        }

        /// <summary>
        /// Retrieves a collection of company references for the authenticated user.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CompanyReference>> List() =>
			GetResource<IEnumerable<CompanyReference>>(MyCompaniesUrls.Root, false);
    }

    internal static class MyCompaniesUrls
    {
        public static string Root => "/veracity/services/v3/my/companies";
    }
}
