using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.Veracity.Services.Api.Directory
{
	public class UserDirectory : ApiResourceClient, IUserDirectory
	{
		public UserDirectory(IHttpClientFactory httpClientFactory, ISerializer serializer, OAuthHttpClientOptions option) : base(httpClientFactory, serializer, option)
		{
		}

		/// <summary>
		/// Retrieves an individual user.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<User> Get(string userId) =>
			GetResource<User>(UserDirectoryUrls.User(userId));

		/// <summary>
		/// Retrieves a collection of users where the id is included in the parameters.
		/// </summary>
		/// <param name="userIds"></param>
		/// <returns></returns>
		public Task<IEnumerable<User>> ListByUserId(params string[] userIds) =>
			PostResource<IEnumerable<User>>(UserDirectoryUrls.Root, ToJsonContent(userIds), false);

		/// <summary>
		/// Retrieves a collection of user references by a specified email value.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> ListByEmail(string email) =>
			GetResource<IEnumerable<UserReference>>(UserDirectoryUrls.UsersByEmail(email), false);

		/// <summary>
		/// Retrieves a collection of company references of companies with which a user is affiliated.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<IEnumerable<CompanyReference>> ListCompanies(string userId) =>
			GetResource<IEnumerable<CompanyReference>>(UserDirectoryUrls.UsersCompanies(userId), false);

		/// <summary>
		/// Retrieves a collection of service references of services to which a user is subscribed.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<ServiceReference>> ListServices(string userId, int page = 1, int pageSize = 20) =>
			GetResource<IEnumerable<ServiceReference>>(UserDirectoryUrls.UsersServices(userId, page, pageSize), false);

		/// <summary>
		/// Retrieve an individual subscription for a specified user and service.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		public Task<Subscription> GetSubscription(string userId, string serviceId) =>
			GetResource<Subscription>(UserDirectoryUrls.UsersServiceSubscription(userId, serviceId));
	}

	internal static class UserDirectoryUrls
	{
		public static string Root => "/veracity/services/v3/directory/users";

		public static string User(string userId) => $"{Root}/{userId}";

		public static string UsersByEmail(string email) => $"{Root}/by/email?email={HttpUtility.UrlEncode(email)}";

		public static string UsersCompanies(string userId) => $"{User(userId)}/companies";

		public static string UsersServices(string userId, int page, int pageSize) => $"{User(userId)}/services?page={page}&pageSize={pageSize}";

		public static string UsersServiceSubscription(string userId, string serviceId) => $"{User(userId)}/services/{serviceId}";
	}
}
