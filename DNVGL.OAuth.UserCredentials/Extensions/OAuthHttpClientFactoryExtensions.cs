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
        /// <param name="name">Value to match <see cref="OAuthHttpClientFactoryOptions.Name">Name</see> of configuration options for the created instance.</param>
        /// <param name="configOverride">an Action which allow to override some configs for the client configuration.</param>
        /// <returns></returns>
        public static System.Net.Http.HttpClient CreateWithUserCredentialFlow(this IOAuthHttpClientFactory factory, string name, Action<OAuthHttpClientFactoryOptions> configOverride = null)
        {
            return factory.Create(c => c.Name == name 
                                       && c.Flow == OAuthCredentialFlow.UserCredentials, configOverride);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name">Value to match <see cref="OAuthHttpClientFactoryOptions.Name">Name</see> of configuration options for the created instance.</param>
        /// <param name="configOverride">an Action which allow to override some configs for the client configuration.</param>
        /// <returns></returns>
        public static System.Net.Http.HttpClient CreateWithClientCredentialFlow(this IOAuthHttpClientFactory factory, string name, Action<OAuthHttpClientFactoryOptions> configOverride = null)
        {
            return factory.Create(c => c.Name == name
                                       && c.Flow == OAuthCredentialFlow.ClientCredentials, configOverride);
        }


    }
}
