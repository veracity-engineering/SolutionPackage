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

		public Task Add(string userId, SubscriptionOptions options) =>
			PutResource(ThisSubscribersUrls.Subscriber(userId), new StringContent(Serialize(options)));

		public Task<UserReference> Get(string userId) =>
			GetResource<UserReference>(ThisSubscribersUrls.Subscriber(userId));

		public Task<IEnumerable<UserReference>> List(int page, int pageSize) =>
			GetResource<IEnumerable<UserReference>>(ThisSubscribersUrls.List(page, pageSize));

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
