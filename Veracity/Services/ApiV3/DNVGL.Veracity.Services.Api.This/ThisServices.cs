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

		public ThisServices(IHttpClientFactory httpClientFactory, ISerializer serializer, OAuthHttpClientOptions option) : base(httpClientFactory, serializer, option)
		{
		}

		/// <summary>
		/// Add a subscription to the authenticated service or nested services.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Task AddSubscription(string serviceId, string userId, SubscriptionOptions options) =>
			PutResource(ThisServicesUrls.ServiceSubscriber(serviceId, userId), new StringContent(Serialize(options)));

		/// <summary>
		/// Retrieve an individual administrator reference to a administrator of the authenticated service or nested services.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
        public Task<AdministratorReference> GetAdministrator(string serviceId, string userId)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Retrieve an individual user reference to a user which has a subscription to a specified service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<UserReference> GetSubscriber(string serviceId, string userId) =>
			GetResource<UserReference>(ThisServicesUrls.ServiceSubscriber(serviceId, userId));

		/// <summary>
		/// Retrieve a collection of services the authenticated service has access to.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<ServiceReference>> List(int page, int pageSize) =>
			GetResource<IEnumerable<ServiceReference>>(ThisServicesUrls.List(page, pageSize), false);

		/// <summary>
		/// Retrieve a collection of administrator references of administrators for a specified service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
        public Task<IEnumerable<AdministratorReference>> ListAdministrators(string serviceId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Retrieve a collection of user references of users subscribed to a specified service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> ListSubscribers(string serviceId, int page, int pageSize) =>
			GetResource<IEnumerable<UserReference>>(ThisServicesUrls.ServiceSubscribers(serviceId, page, pageSize), false);

		/// <summary>
		/// Send a notification to users subscribed to the authenticated service or nested service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
        public Task NotifySubscribers(string serviceId, NotificationOptions options)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Remove a user subscription for a user and the authenticated service or a nested service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
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
