using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.Veracity.Services.Api.This
{
	public class ThisUsers : ApiClientBase, IThisUsers
    {		
		public ThisUsers(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
			: base(optionsList, httpClientFactory, serializer)
		{
		}

		/// <summary>
		/// Create a new user.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public async Task<CreateUserReference> Create(CreateUserOptions options)
		{
			var client = base.GetClient();
			return await client.PostResource<CreateUserReference>(ThisUsersUrls.UserRoot, client.ToJsonContent(options));
		}
		/// <summary>
		/// Create a collection of new users.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public Task<IEnumerable<CreateUserReference>> Create(params CreateUserOptions[] options)
		{
			var client = base.GetClient();
			return client.PostResource<IEnumerable<CreateUserReference>>(ThisUsersUrls.UsersRoot, client.ToJsonContent(options));
		}
		/// <summary>
		/// Retrieves a collection of user references for users with a specified email value.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> Resolve(string email) =>
			base.GetClient().GetResource<IEnumerable<UserReference>>(ThisUsersUrls.Resolve(email));
    }

    internal static class ThisUsersUrls
    {
        public static string UsersRoot => "/veracity/services/v3/this/users";

        public static string UserRoot => "/veracity/services/v3/this/user";

        public static string Resolve(string email) => $"{UserRoot}/resolve({HttpUtility.UrlEncode(email)})";
    }
}
