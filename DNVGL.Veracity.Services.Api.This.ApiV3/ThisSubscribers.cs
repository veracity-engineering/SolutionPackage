using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.ApiV3
{
    public class ThisSubscribers : ApiResourceClient, IThisSubscribers
    {
        public ThisSubscribers(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task Add(string userId, SubscriptionOptions options)
        {
            var response = await GetOrCreateHttpClient().PutAsync(ThisSubscribersUrls.Subscriber(userId), new StringContent(Serialize(options)));
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }

        public async Task<UserReference> Get(string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisSubscribersUrls.Subscriber(userId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<UserReference>(content);
        }

        public async Task<IEnumerable<UserReference>> List(int page, int pageSize)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisSubscribersUrls.List(page, pageSize));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<UserReference>>(content);
        }

        public async Task Remove(string userId)
        {
            var response = await GetOrCreateHttpClient().DeleteAsync(ThisSubscribersUrls.Subscriber(userId));
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }
    }

    internal class ThisSubscribersUrls
    {
        public static string Root => "/veracity/services/v3/this/subscribers";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string Subscriber(string userId) => $"{Root}/{userId}";
    }
}
