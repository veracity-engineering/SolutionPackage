using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.Abstractions
{
	/// <summary>
	/// APIV3: User Directory 
	/// </summary>
	public interface IUserDirectory
    {
		/// <summary>
		/// Returns the user by user id.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<User> Get(string userId);

		/// <summary>
		/// Returns a collection of users by a collection of user ids.
		/// </summary>
		/// <param name="userIds"></param>
		/// <returns></returns>
		Task<IEnumerable<User>> ListByUserId(params string[] userIds);

		/// <summary>
		/// Returns a collection of users by email.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		Task<IEnumerable<UserReference>> ListByEmail(string email);

		/// <summary>
		/// Returns a collection of companies a user is affiliated with.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<IEnumerable<CompanyReference>> ListCompanies(string userId);

		/// <summary>
		/// Returns a collection of services a user is subscribed to.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<IEnumerable<ServiceReference>> ListServices(string userId, int page = 1, int pageSize = 20);

		/// <summary>
		/// Returns a user's subscription to a service by user id and service id.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		Task<Subscription> GetSubscription(string userId, string serviceId);


		//2022-05-30
		//https://api-test-portal.veracity.com/docs/services/Veracity%20-%20MyServices%20V3/operations/5c7fc936deb9ff096009b5b4?tags=&pattern=directory&groupBy=
		//todo: implemnet these apis
		//Task AcceptTerms();
		//Task ActivateUserAccount();
		//Task DeleteUser(string userId);
		//Task ExchangeOtpCode();
		//Task GetPendingUserActivation();
		//Task GetUserResyncData(string userId);
		//Task UpdateCurrentUser();
		//Task UpdateCurrentUsersEmailOrPhone(string type);
		//Task UpdateCurrentUsersPassword();
		//Task UpdateUserEmail(string userId);
		//Task ValidateEmailOrPhone(string type);
	}
}
;