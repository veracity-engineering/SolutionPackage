﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DNVGL.Veracity.Services.Api.This
{
    public class ThisUsers : ApiResourceClient, IThisUsers
    {
        public ThisUsers(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

		public Task<CreateUserReference> Create(CreateUserOptions options) =>
			PostResource<CreateUserReference>(ThisUsersUrls.UserRoot, new StringContent(Serialize(options)));

		public Task<IEnumerable<CreateUserReference>> Create(params CreateUserOptions[] options) =>
			PostResource<IEnumerable<CreateUserReference>>(ThisUsersUrls.UsersRoot, new StringContent(Serialize(options)));

		public Task<IEnumerable<UserReference>> Resolve(string email) =>
			GetResource<IEnumerable<UserReference>>(ThisUsersUrls.Resolve(email));
    }

    internal static class ThisUsersUrls
    {
        public static string UsersRoot => "/veracity/services/v3/this/users";

        public static string UserRoot => "/veracity/services/v3/this/user";

        public static string Resolve(string email) => $"{UserRoot}/resolve({HttpUtility.UrlEncode(email)})";
    }
}
