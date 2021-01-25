using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public interface IThisAdministrators
    {
        Task<IEnumerable<AdministratorReference>> List(int page, int pageSize);

        Task<Administrator> Get(string userId);
    }
}
