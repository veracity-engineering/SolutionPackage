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
		public UserDirectory(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
		{
		}

		public Task<User> Get(string userId) =>
			GetResult<User>(UserDirectoryUrls.User(userId));

		public Task<IEnumerable<User>> ListByUserId(params string[] userIds) =>
			PostResult<IEnumerable<User>>(UserDirectoryUrls.Root, new StringContent(Serialize(userIds)), false);

		public Task<IEnumerable<UserReference>> ListByEmail(string email) =>
			GetResult<IEnumerable<UserReference>>(UserDirectoryUrls.UsersByEmail(email), false);

		public Task<IEnumerable<CompanyReference>> ListCompanies(string userId) =>
			GetResult<IEnumerable<CompanyReference>>(UserDirectoryUrls.UsersCompanies(userId), false);

		public Task<IEnumerable<ServiceReference>> ListServices(string userId, int page = 1, int pageSize = 20) =>
			GetResult<IEnumerable<ServiceReference>>(UserDirectoryUrls.UsersServices(userId, page, pageSize), false);

		public Task<Subscription> GetSubscription(string userId, string serviceId) =>
			GetResult<Subscription>(UserDirectoryUrls.UsersServiceSubscription(userId, serviceId));
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
