using DNVGL.Veracity.Services.Api.Models;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
    public interface IMyProfile
    {
		/// <summary>
		/// Returns profile of authenticated user.
		/// </summary>
		/// <returns></returns>
		Task<Profile> Get();
    }
}
