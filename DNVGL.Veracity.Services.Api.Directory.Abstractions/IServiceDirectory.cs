using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.Abstractions
{
    public interface IServiceDirectory
    {
        Task<Service> Get(string serviceId);

        Task<IEnumerable<UserReference>> ListUsers(string serviceId, int page = 1, int pageSize = 20);
    }
}
