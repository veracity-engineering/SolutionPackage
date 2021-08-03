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
            {
                if (config.Flow != OAuthCredentialFlow.UserCredentials)
                {
                    if (!(config.OAuthClientOptions?.Scopes?.Any() ?? false))
                        throw new ArgumentNullException($"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.Scopes)}");
                    
                    config.Flow = OAuthCredentialFlow.UserCredentials;
                }
            });
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
            {
                if (config.Flow != OAuthCredentialFlow.ClientCredentials)
                {
                    if (string.IsNullOrEmpty(config.OAuthClientOptions?.ClientId))
                        throw new ArgumentNullException($"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.ClientId)}");

                    if (string.IsNullOrEmpty(config.OAuthClientOptions?.ClientSecret))
                        throw new ArgumentNullException($"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.ClientSecret)}");
                    
                    if (!(config.OAuthClientOptions?.Scopes?.Any()??false))
                        throw new ArgumentNullException($"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.Scopes)}");

                    if (string.IsNullOrEmpty(config.OAuthClientOptions?.Authority))
                        throw new ArgumentNullException($"{nameof(config.OAuthClientOptions)}.{nameof(config.OAuthClientOptions.Authority)}");

                    config.Flow = OAuthCredentialFlow.ClientCredentials;
                }
            });
        }


    }
}
