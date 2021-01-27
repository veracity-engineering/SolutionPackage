using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.ApiV3
{
    public class ThisServices : ApiResourceClient, IThisServices
    {
        private const string HttpClientConfigurationName = "services-this-api";

        public ThisServices(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName = HttpClientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task AddSubscription(string serviceId, string userId, SubscriptionOptions options)
        {
            var response = await GetOrCreateHttpClient().PutAsync(ThisServicesUrls.ServiceSubscriber(serviceId, userId), new StringContent(Serialize(options)));
            object p = response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }

        public Task<AdministratorReference> GetAdministrator(string serviceId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserReference> GetSubscriber(string serviceId, string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisServicesUrls.ServiceSubscriber(serviceId, userId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<UserReference>(content);
        }

        public async Task<IEnumerable<ServiceReference>> List(int page, int pageSize)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisServicesUrls.List(page, pageSize));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<ServiceReference>>(content);
        }

        public Task<IEnumerable<AdministratorReference>> ListAdministrators(string serviceId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserReference>> ListSubscribers(string serviceId, int page, int pageSize)
        {
            var response = await GetOrCreateHttpClient().GetAsync(ThisServicesUrls.ServiceSubscribers(serviceId, page, pageSize));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<UserReference>>(content);
        }

        public Task NotifySubscribers(string serviceId, NotificationOptions options)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveSubscription(string serviceId, string userId)
        {
            var response = await GetOrCreateHttpClient().DeleteAsync(ThisServicesUrls.ServiceSubscriber(serviceId, userId));
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }
    }

    internal class ThisServicesUrls
    {
        public static string Root => "/veracity/services/v3/this/services";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string ServiceSubscribers(string serviceId, int page, int pageSize) => $"{Root}/{serviceId}/subscribers?page={page}&pageSize={pageSize}";

        public static string ServiceSubscriber(string serviceId, string userId) => $"{Root}/{serviceId}/subscribers/{userId}";

        public static string Notify(string serviceId) => $"{Root}/{serviceId}/notification";
    }
}
