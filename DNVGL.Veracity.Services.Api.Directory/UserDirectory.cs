﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.Veracity.Services.Api.Directory
{
    public class UserDirectory : ApiResourceClient, IUserDirectory
    {
        public UserDirectory(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<User> Get(string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.User(userId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<User>(content);
        }

        public async Task<IEnumerable<User>> ListByUserId(params string[] userIds)
        {
            var response = await GetOrCreateHttpClient().PostAsync(UserDirectoryUrls.Root, new StringContent(Serialize(userIds)));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<User>>(content);
        }

        public async Task<IEnumerable<UserReference>> ListByEmail(string email)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersByEmail(email));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<UserReference>>(content);
        }

        public async Task<IEnumerable<CompanyReference>> ListCompanies(string userId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersCompanies(userId));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<CompanyReference>>(content);
        }

        public async Task<IEnumerable<ServiceReference>> ListServices(string userId, int page = 1, int pageSize = 20)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersServices(userId, page, pageSize));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<ServiceReference>>(content);
        }

        public async Task<Subscription> GetSubscription(string userId, string serviceId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(UserDirectoryUrls.UsersServiceSubscription(userId, serviceId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Subscription>(content);
        }
    }

    internal static class UserDirectoryUrls
    {
        public static string Root => "/veracity/services/v3/directory/users";

        public static string User(string userId) => $"{Root}/{userId}";

        public static string UsersByEmail(string email) => $"{Root}/by/email?email={HttpUtility.UrlEncode(email)}";

        public static string UsersCompanies(string userId) => $"{User(userId)}/companies";

        public static string UsersServices(string userId, int page, int pageSize) => $"{User(userId)}/services?page={page}&pageSize={pageSize}";

        public static string UsersServiceSubscription(string userId, string serviceId) => $"{User(userId)}/services/{serviceId}";
    }
}
