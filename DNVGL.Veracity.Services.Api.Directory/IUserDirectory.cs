using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
    public interface IUserDirectory
    {
        Task<User> Get(string userId);

        Task<IEnumerable<User>> ListByUserId(params string[] userIds);

        Task<IEnumerable<UserReference>> ListByEmail(string email);

        Task<IEnumerable<CompanyReference>> ListCompanies(string userId);

        Task<IEnumerable<ServiceReference>> ListServices(string userId, int page = 1, int pageSize = 20);

        Task<Subscription> GetSubscription(string userId, string serviceId);
    }
}
