using DNVGL.Veracity.Services.Api.Models;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
    public interface IMyProfile
    {
        Task<Profile> Get();
    }
}
