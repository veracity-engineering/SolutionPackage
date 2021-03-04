using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.Abstractions
{
    public interface IThisSubscribers
    {
        Task<IEnumerable<UserReference>> List(int page, int pageSize);

        Task<UserReference> Get(string userId);

        Task Add(string userId, SubscriptionOptions options);

        Task Remove(string userId);
    }
}