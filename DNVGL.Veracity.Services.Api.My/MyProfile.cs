using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyProfile : ApiResourceClient, IMyProfile
    {
        public MyProfile(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

		public Task<Profile> Get() =>
			GetResource<Profile>(MyProfileUrls.Root);
    }

    internal static class MyProfileUrls
    {
        public static string Root => "/veracity/services/v3/my/profile";
    }
}
