﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
	public class CompanyDirectory :ApiClientBase,  ICompanyDirectory
	{
		public CompanyDirectory(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
		   : base(optionsList, httpClientFactory, serializer)
		{

		}

		/// <summary>
		/// Retrieves an individual company.
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		public Task<Company> Get(string companyId) =>
			base.GetClient().GetResource<Company>(CompanyDirectoryUrls.Company(companyId));

		/// <summary>
		/// Retrieves a paginated collection of user references of users affiliated with a company.
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> ListUsers(string companyId, int page = 1, int pageSize = 20) =>
			base.GetClient().GetResource<IEnumerable<UserReference>>(CompanyDirectoryUrls.CompanyUsers(companyId, page, pageSize), false);
	}

	internal static class CompanyDirectoryUrls
	{
		public static string Root => "/veracity/services/v3/directory/companies";

		public static string Company(string companyId) => $"{Root}/{companyId}";

		public static string CompanyUsers(string companyId, int page, int pageSize) => $"{Company(companyId)}/users?page={page}&pageSize={pageSize}";
	}
}
