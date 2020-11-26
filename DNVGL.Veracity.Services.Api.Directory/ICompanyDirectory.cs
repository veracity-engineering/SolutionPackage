using DNVGL.Veracity.Services.Api.Models;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory
{
    public interface ICompanyDirectory
    {
        Task<Company> Get(string companyId);
    }
}
