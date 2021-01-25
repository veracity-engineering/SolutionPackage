using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public interface IThisSubscribers
    {
        Task<IEnumerable<UserReference>> ListSubscribers();

        Task<UserReference> GetSubscriber(string userId);

        Task AddSubscriber(string userId, SubscriptionOptions options);

        Task RemoveSubscriber(string userId);
    }
}
