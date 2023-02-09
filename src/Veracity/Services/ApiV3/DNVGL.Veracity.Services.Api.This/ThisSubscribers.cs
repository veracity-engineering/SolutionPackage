using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisSubscribers : IThisSubscribers
    {
        private readonly ApiClientFactory _apiClientFactory;
        public ThisSubscribers(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        /// <summary>
        /// Add a subscription to the authenticated service for a specified user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task Add(string userId, SubscriptionOptions options)
		{
			var client = _apiClientFactory.GetClient();
			await client.PutResource(ThisSubscribersUrls.Subscriber(userId), client.ToJsonContent(options));
		}
			

		/// <summary>
		/// Retrieve a user reference for a user subscribed to the authenticated service.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<UserReference> Get(string userId) =>
            _apiClientFactory.GetClient().GetResource<UserReference>(ThisSubscribersUrls.Subscriber(userId));

		/// <summary>
		/// Retrieve a collection of user references to users subscribed to the authenticated service.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> List(int page, int pageSize) =>
            _apiClientFactory.GetClient().GetResource<IEnumerable<UserReference>>(ThisSubscribersUrls.List(page, pageSize));

		/// <summary>
		/// Remove a user subscription to the authenticated service by specified user.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task Remove(string userId) =>
            _apiClientFactory.GetClient().DeleteResource(ThisSubscribersUrls.Subscriber(userId));
    }

    internal static class ThisSubscribersUrls
    {
        public static string Root => "/veracity/services/v3/this/subscribers";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string Subscriber(string userId) => $"{Root}/{userId}";
    }
}
