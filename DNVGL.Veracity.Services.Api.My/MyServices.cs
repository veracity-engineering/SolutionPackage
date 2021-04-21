﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyServices : ApiResourceClient, IMyServices
    {
        public MyServices(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<IEnumerable<MyServiceReference>> List()
        {
            var response = await GetOrCreateHttpClient().GetAsync(MyServicesUrls.Root);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<MyServiceReference>>(content);
        }
    }

    internal static class MyServicesUrls
    {
        public static string Root => "/veracity/services/v3/my/services";
    }
}
