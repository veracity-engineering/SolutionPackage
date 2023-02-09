using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.Veracity.Services.Api.This
{
	public class ThisUsers :  IThisUsers
    {
        private readonly ApiClientFactory _apiClientFactory;
        public ThisUsers(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<CreateUserReference> Create(CreateUserOptions options)
		{
			var client = _apiClientFactory.GetClient();
			return await client.PostResource<CreateUserReference>(ThisUsersUrls.UserRoot, client.ToJsonContent(options));
		}
		/// <summary>
		/// Create a collection of new users.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public Task<IEnumerable<CreateUserReference>> Create(params CreateUserOptions[] options)
		{
			var client = _apiClientFactory.GetClient();
			return client.PostResource<IEnumerable<CreateUserReference>>(ThisUsersUrls.UsersRoot, client.ToJsonContent(options));
		}
		/// <summary>
		/// Retrieves a collection of user references for users with a specified email value.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> Resolve(string email) =>
            _apiClientFactory.GetClient().GetResource<IEnumerable<UserReference>>(ThisUsersUrls.Resolve(email));
    }

    internal static class ThisUsersUrls
    {
        public static string UsersRoot => "/veracity/services/v3/this/users";

        public static string UserRoot => "/veracity/services/v3/this/user";

        public static string Resolve(string email) => $"{UserRoot}/resolve({HttpUtility.UrlEncode(email)})";
    }
}
