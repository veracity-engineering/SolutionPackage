using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
	public class ThisAdministrators : ApiClientBase, IThisAdministrators
	{
		public ThisAdministrators(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
			: base(optionsList, httpClientFactory, serializer)
		{

		}

		/// <summary>
		/// Retrieves an individual administrator for the authenticated service.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<Administrator> Get(string userId) =>
			base.GetClient().GetResource<Administrator>(ThisAdministratorsUrls.Administrator(userId));

		/// <summary>
		/// Retrieves a collection of administrator references for the authenticated service.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<AdministratorReference>> List(int page, int pageSize) =>
			base.GetClient().GetResource<IEnumerable<AdministratorReference>>(ThisAdministratorsUrls.List(page, pageSize));
	}

	internal static class ThisAdministratorsUrls
	{
		public static string Root => "/veracity/services/v3/this/administrators";

		public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

		public static string Administrator(string userId) => $"{Root}/{userId}";
	}
}
