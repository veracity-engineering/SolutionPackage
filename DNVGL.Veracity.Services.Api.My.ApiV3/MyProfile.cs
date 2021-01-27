﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.ApiV3
{
    public class MyProfile : ApiResourceClient, IMyProfile
    {
        public MyProfile(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<Profile> Get()
        {
            var response = await GetOrCreateHttpClient().GetAsync(MyProfileUrls.Root);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Profile>(content);
        }
    }

    internal class MyProfileUrls
    {
        public static string Root => "/veracity/services/v3/my/profile";
    }
}
