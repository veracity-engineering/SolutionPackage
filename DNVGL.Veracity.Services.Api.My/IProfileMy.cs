using DNVGL.Veracity.Services.Api.Models;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public interface IProfileMy
    {
        Task<Profile> Get();
    }
}
