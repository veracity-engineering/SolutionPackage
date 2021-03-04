using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.Abstractions
{
    public interface ICompanyDirectory
    {
        Task<Company> Get(string companyId);

        Task<IEnumerable<UserReference>> ListUsers(string companyId, int page = 1, int pageSize = 20);
    }
}
