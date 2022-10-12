using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyCompanies :  IMyCompanies
    {
        private readonly ApiClientFactory _apiClientFactory;
        public MyCompanies(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        /// <summary>
        /// Retrieves a collection of company references for the authenticated user.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CompanyReference>> List() =>
            _apiClientFactory.GetClient().GetResource<IEnumerable<CompanyReference>>(MyCompaniesUrls.Root, false);
    }

    internal static class MyCompaniesUrls
    {
        public static string Root => "/veracity/services/v3/my/companies";
    }
}
