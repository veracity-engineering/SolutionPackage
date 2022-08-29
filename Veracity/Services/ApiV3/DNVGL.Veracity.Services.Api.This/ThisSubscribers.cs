using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisSubscribers : ApiClientBase, IThisSubscribers
    {
		public ThisSubscribers(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
			: base(optionsList, httpClientFactory, serializer)
		{

		}

		/// <summary>
		/// Add a subscription to the authenticated service for a specified user.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public async Task Add(string userId, SubscriptionOptions options)
		{
			var client = base.GetClient();
			await client.PutResource(ThisSubscribersUrls.Subscriber(userId), client.ToJsonContent(options));
		}
			

		/// <summary>
		/// Retrieve a user reference for a user subscribed to the authenticated service.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<UserReference> Get(string userId) =>
			base.GetClient().GetResource<UserReference>(ThisSubscribersUrls.Subscriber(userId));

		/// <summary>
		/// Retrieve a collection of user references to users subscribed to the authenticated service.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> List(int page, int pageSize) =>
			base.GetClient().GetResource<IEnumerable<UserReference>>(ThisSubscribersUrls.List(page, pageSize));

		/// <summary>
		/// Remove a user subscription to the authenticated service by specified user.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task Remove(string userId) =>
			base.GetClient().DeleteResource(ThisSubscribersUrls.Subscriber(userId));
    }

    internal static class ThisSubscribersUrls
    {
        public static string Root => "/veracity/services/v3/this/subscribers";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string Subscriber(string userId) => $"{Root}/{userId}";
    }
}
