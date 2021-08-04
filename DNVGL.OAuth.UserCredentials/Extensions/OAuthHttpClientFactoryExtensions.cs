using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DNVGL.OAuth.Web.Abstractions;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
    public static class OAuthHttpClientFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Net.Http.HttpClient CreateWithUserCredentialFlow(this IOAuthHttpClientFactory factory, string name)
        {
            return factory.Create(name, config =>
                config.Flow = OAuthCredentialFlow.UserCredentials);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Net.Http.HttpClient CreateWithClientCredentialFlow(this IOAuthHttpClientFactory factory, string name)
        {
            return factory.Create(name, config => 
                config.Flow = OAuthCredentialFlow.ClientCredentials);
        }


    }
}
