using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisServices : ApiResourceClient, IThisServices
    {
        private const string HttpClientConfigurationName = "services-this-api";

        public ThisServices(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName = HttpClientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

		public Task AddSubscription(string serviceId, string userId, SubscriptionOptions options) =>
			PutResource(ThisServicesUrls.ServiceSubscriber(serviceId, userId), new StringContent(Serialize(options)));

        public Task<AdministratorReference> GetAdministrator(string serviceId, string userId)
        {
            throw new NotImplementedException();
        }

		public Task<UserReference> GetSubscriber(string serviceId, string userId) =>
			GetResource<UserReference>(ThisServicesUrls.ServiceSubscriber(serviceId, userId));

		public Task<IEnumerable<ServiceReference>> List(int page, int pageSize) =>
			GetResource<IEnumerable<ServiceReference>>(ThisServicesUrls.List(page, pageSize), false);

        public Task<IEnumerable<AdministratorReference>> ListAdministrators(string serviceId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

		public Task<IEnumerable<UserReference>> ListSubscribers(string serviceId, int page, int pageSize) =>
			GetResource<IEnumerable<UserReference>>(ThisServicesUrls.ServiceSubscribers(serviceId, page, pageSize), false);

        public Task NotifySubscribers(string serviceId, NotificationOptions options)
        {
            throw new NotImplementedException();
        }

		public Task RemoveSubscription(string serviceId, string userId) =>
			DeleteResource(ThisServicesUrls.ServiceSubscriber(serviceId, userId));
    }

    internal static class ThisServicesUrls
    {
        public static string Root => "/veracity/services/v3/this/services";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string ServiceSubscribers(string serviceId, int page, int pageSize) => $"{Root}/{serviceId}/subscribers?page={page}&pageSize={pageSize}";

        public static string ServiceSubscriber(string serviceId, string userId) => $"{Root}/{serviceId}/subscribers/{userId}";

        public static string Notify(string serviceId) => $"{Root}/{serviceId}/notification";
    }
}
