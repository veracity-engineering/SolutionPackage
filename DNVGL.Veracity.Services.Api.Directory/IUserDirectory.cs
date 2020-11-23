using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
    public interface IUserDirectory
    {
        Task<User> Get(string userId);

        Task Delete(string userId);

        Task<IEnumerable<User>> ListByEmail(string email);

        Task<IEnumerable<CompanyReference>> ListCompanies(string userId);

        Task<IEnumerable<ServiceReference>> ListServices(string userId);

        Task<Subscription> GetSubscription(string userId, string serviceId);
    }
}
