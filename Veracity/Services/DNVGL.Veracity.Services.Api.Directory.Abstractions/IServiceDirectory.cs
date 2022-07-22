using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.Abstractions
{
	/// <summary>
	/// APIV3: Services Directory
	/// </summary>
    public interface IServiceDirectory
    {
		/// <summary>
		/// Returns the service by service id.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		Task<Service> Get(string serviceId);

		/// <summary>
		/// Returns a collection of users subscribed to the service by service id.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<IEnumerable<UserReference>> ListUsers(string serviceId, int page = 1, int pageSize = 20);



		Task<IEnumerable<Subscription>> GetServiceSubscriptions(string serviceId, string filter, string pageNo);

		Task<bool> IsAdmin(string serviceId, string userId);

	}
}
