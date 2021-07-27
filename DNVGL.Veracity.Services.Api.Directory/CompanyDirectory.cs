using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
	public class CompanyDirectory : ApiResourceClient, ICompanyDirectory
	{
		public CompanyDirectory(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
		{
		}

		public Task<Company> Get(string companyId) =>
			GetResource<Company>(CompanyDirectoryUrls.Company(companyId));

		public Task<IEnumerable<UserReference>> ListUsers(string companyId, int page = 1, int pageSize = 20) =>
			GetResource<IEnumerable<UserReference>>(CompanyDirectoryUrls.CompanyUsers(companyId, page, pageSize), false);
	}

	internal static class CompanyDirectoryUrls
	{
		public static string Root => "/veracity/services/v3/directory/companies";

		public static string Company(string companyId) => $"{Root}/{companyId}";

		public static string CompanyUsers(string companyId, int page, int pageSize) => $"{Company(companyId)}/users?page={page}&pageSize={pageSize}";
	}
}
