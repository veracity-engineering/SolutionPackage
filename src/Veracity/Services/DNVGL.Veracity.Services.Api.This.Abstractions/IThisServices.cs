using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.Abstractions
{
	// TODO: Some methods stil undocumented and unimplemented
	public interface IThisServices
	{
		/// <summary>
		/// Returns a collection of services the authenticated company is subscribed to.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<IEnumerable<ServiceReference>> List(int page, int pageSize);

		/// <summary>
		/// Returns a collection of users inheriting a subscription to a service from the authenticated company.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<IEnumerable<UserReference>> ListSubscribers(string serviceId, int page, int pageSize);

		/// <summary>
		/// Returns a user who has inherited a service subscription from the authenticated company.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<UserReference> GetSubscriber(string serviceId, string userId);

		/// <summary>
		/// Add a subscription to a user which is inherited by the authenticated company. 
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Task AddSubscription(string serviceId, string userId, SubscriptionOptions options);

		/// <summary>
		/// Remove a user's subscription to a service inherited by the authenticated company.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task RemoveSubscription(string serviceId, string userId);

		Task<IEnumerable<AdministratorReference>> ListAdministrators(string serviceId, int page, int pageSize);

		Task<Administrator> GetAdministrator(string serviceId, string userId);

		Task NotifySubscribers(string serviceId, string channelId, NotificationOptions options);

		Task<PolicyValidationResult> VerifySubscriberPolicy(string serviceId, string userId, string returnUrl = null);


		Task<ProfilePicture> GetProfilePicture(string serviceId, string userId);
	}
}
