using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisServices :ApiClientBase, IThisServices
    {
        private const string HttpClientConfigurationName = "services-this-api";
		
		public ThisServices(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
			: base(optionsList, httpClientFactory, serializer)
		{

		}

		/// <summary>
		/// Add a subscription to the authenticated service or nested services.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public async Task AddSubscription(string serviceId, string userId, SubscriptionOptions options) {
			var client = base.GetClient();
			await client.PutResource(ThisServicesUrls.ServiceSubscriber(serviceId, userId), client.ToJsonContent(options));
		}
			

		/// <summary>
		/// Retrieve an individual administrator reference to a administrator of the authenticated service or nested services.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
        public Task<Administrator> GetAdministrator(string serviceId, string userId)
			=> base.GetClient().GetResource<Administrator>(ThisServicesUrls.GetAdmin(serviceId, userId));		

		/// <summary>
		/// Retrieve an individual user reference to a user which has a subscription to a specified service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<UserReference> GetSubscriber(string serviceId, string userId) =>
			base.GetClient().GetResource<UserReference>(ThisServicesUrls.ServiceSubscriber(serviceId, userId));

		/// <summary>
		/// Retrieve a collection of services the authenticated service has access to.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<ServiceReference>> List(int page, int pageSize) =>
			base.GetClient().GetResource<IEnumerable<ServiceReference>>(ThisServicesUrls.List(page, pageSize), false);

		/// <summary>
		/// Retrieve a collection of administrator references of administrators for a specified service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
        public Task<IEnumerable<AdministratorReference>> ListAdministrators(string serviceId, int page, int pageSize) =>
			base.GetClient().GetResource<IEnumerable<AdministratorReference>>(ThisServicesUrls.GetAdmins(serviceId, page, pageSize), false);

		/// <summary>
		/// Retrieve a collection of user references of users subscribed to a specified service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public Task<IEnumerable<UserReference>> ListSubscribers(string serviceId, int page, int pageSize) =>
			base.GetClient().GetResource<IEnumerable<UserReference>>(ThisServicesUrls.ServiceSubscribers(serviceId, page, pageSize), false);

		/// <summary>
		/// Send a notification to users subscribed to the authenticated service or nested service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="channelId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
        public async Task NotifySubscribers(string serviceId, string channelId, NotificationOptions options)
		{
			var client = base.GetClient();

			var request = new HttpRequestMessage(HttpMethod.Post, ThisServicesUrls.Notify(serviceId)) { 
				 Content = client.ToJsonContent(options)
			};

			if (!string.IsNullOrEmpty(channelId))
				request.Headers.Add("channelId", channelId);

			await client.ToResourceResult(request);
		}

		/// <summary>
		/// Remove a user subscription for a user and the authenticated service or a nested service.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task RemoveSubscription(string serviceId, string userId) =>
			base.GetClient().DeleteResource(ThisServicesUrls.ServiceSubscriber(serviceId, userId));



		/// <summary>
		///		verify policy
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		public async Task<PolicyValidationResult> VerifySubscriberPolicy(string serviceId, string userId, string returnUrl = null)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, ThisServicesUrls.VerifySubscriberPolicy(serviceId, userId));
			if (!string.IsNullOrEmpty(returnUrl))
				request.Headers.Add("returnUrl", returnUrl);

			return await base.GetClient().ToResourceResult<PolicyValidationResult>(request);
		}

		/// <summary>
		///		Get user picture
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public Task<ProfilePicture> GetProfilePicture(string serviceId, string userId) =>
		 base.GetClient().GetResource<ProfilePicture>(ThisServicesUrls.GetProfilePicture(serviceId, userId), isNotFoundNull: true);
	}

    internal static class ThisServicesUrls
    {
        public static string Root => "/veracity/services/v3/this/services";

        public static string List(int page, int pageSize) => $"{Root}?page={page}&pageSize={pageSize}";

        public static string ServiceSubscribers(string serviceId, int page, int pageSize) => $"{Root}/{serviceId}/subscribers?page={page}&pageSize={pageSize}";

        public static string ServiceSubscriber(string serviceId, string userId) => $"{Root}/{serviceId}/subscribers/{userId}";

        public static string Notify(string serviceId) => $"{Root}/{serviceId}/notification";

		public static string VerifySubscriberPolicy(string serviceId, string userId) => $"{Root}/{serviceId}/subscribers/{userId}/policy/validate()";


		public static string GetAdmin(string serviceId, string userId) => $"{Root}/{serviceId}/administrators/{userId}";

		public static string GetAdmins(string serviceId,int page, int pageSize) => $"{Root}/{serviceId}/administrators?page={page}&pageSize={pageSize}";

		public static string GetProfilePicture(string serviceId, string userId) => $"{Root}/{serviceId}/subscribers/{userId}/picture";

	}
}
