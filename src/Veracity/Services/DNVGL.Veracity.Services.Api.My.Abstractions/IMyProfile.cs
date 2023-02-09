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

		/// <summary>
		///	Retreives the profile picture of the current logegd in user if one is set, otherwise a 404 is returned
		/// </summary>
		/// <returns></returns>
		Task<ProfilePicture> GetProfilePicture();
    }
}
