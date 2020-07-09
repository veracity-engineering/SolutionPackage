using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DNVGL.OAuth.UserCredentials
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddOAuthHttpClientFactory(this AuthenticationBuilder authBuilder, Action<ICollection<OAuthHttpClientFactoryOptions>> configureOptions)
        {
            var options = new List<OAuthHttpClientFactoryOptions>();
            configureOptions(options);
            authBuilder.Services.AddSingleton<IOAuthHttpClientFactory>(s => new OAuthHttpClientFactory(options, s.GetService<IHttpContextAccessor>()));
            return authBuilder;
        }
    }
}
