using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public interface IMyCompanies
    {
        Task<IEnumerable<CompanyReference>> List();
    }
}
