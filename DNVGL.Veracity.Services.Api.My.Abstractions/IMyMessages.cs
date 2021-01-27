using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
    public interface IMyMessages
    {
        Task<IEnumerable<Message>> List(bool includeRead = false);

        Task<Message> Get(string messageId);

        Task<int> GetUnreadCount();
    }
}
