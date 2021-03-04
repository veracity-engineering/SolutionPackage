using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.Abstractions
{
    public interface IThisServices
    {
        Task<IEnumerable<ServiceReference>> List(int page, int pageSize);

        Task<IEnumerable<UserReference>> ListSubscribers(string serviceId, int page, int pageSize);

        Task<UserReference> GetSubscriber(string serviceId, string userId);

        Task AddSubscription(string serviceId, string userId, SubscriptionOptions options);

        Task RemoveSubscription(string serviceId, string userId);

        Task<IEnumerable<AdministratorReference>> ListAdministrators(string serviceId, int page, int pageSize);

        Task<AdministratorReference> GetAdministrator(string serviceId, string userId);

        Task NotifySubscribers(string serviceId, NotificationOptions options);
    }
}
