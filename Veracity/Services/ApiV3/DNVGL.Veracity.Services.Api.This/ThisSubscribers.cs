using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisSubscribers : ApiResourceClient, IThisSubscribers
    {
        public ThisSubscribers(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

		/// <summary>
		/// Add a subscription to the authenticated service for a specified user.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Task Add(string userId, SubscriptionOptions options) =>
			PutResource(ThisSubscribersUrls.Subscriber(userId), new StringContent(Serialize(options)));

		/// <summary>
		/// Retrieve a user reference for a user subscribed to the authenticated service.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<UserReference> Get(string userId) =>
			GetResource<UserReference>(ThisSubscribersUrls.Subscriber(userId));

		/// <summary>
		/// Retrieve a collection of user references to users subscribed to the authenticated service.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> List(int page, int pageSize) =>
			GetResource<IEnumerable<UserReference>>(ThisSubscribersUrls.List(page, pageSize));

		/// <summary>
		/// Remove a user subscription to the authenticated service by specified user.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task Remove(string userId) =>
			DeleteResource(ThisSubscribersUrls.Subscriber(userId));
    }

    internal static class ThisSubscribersUrls
    {
        public static string Root => "/veracity/services/v3/this/subscribers";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string Subscriber(string userId) => $"{Root}/{userId}";
    }
}
