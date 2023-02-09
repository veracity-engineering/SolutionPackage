using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyProfile : IMyProfile 
    {
        private readonly ApiClientFactory _apiClientFactory;
        public MyProfile(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        /// <summary>
        /// Retrieves the user profile for the authenticated user.
        /// </summary>
        /// <returns></returns>
        public Task<Profile> Get() =>
            _apiClientFactory.GetClient().GetResource<Profile>(MyProfileUrls.Root);		

        /// <summary>
		/// Retreives the profile picture of the current logegd in user if one is set, otherwise a 404 is returned
		/// </summary>
		/// <returns></returns>
		public Task<ProfilePicture> GetProfilePicture() =>
            _apiClientFactory.GetClient().GetResource<ProfilePicture>(MyProfileUrls.ProfilePicture, isNotFoundNull: true);
        
    }

    internal static class MyProfileUrls
    {
        public static string Root => "/veracity/services/v3/my";

        public static string Profile => $"{Root}/profile";
        public static string ProfilePicture => $"{Root}/picture";
    }
}
