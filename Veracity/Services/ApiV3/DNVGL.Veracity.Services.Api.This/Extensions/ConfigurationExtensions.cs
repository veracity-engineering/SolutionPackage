using DNVGL.OAuth.Api.HttpClient;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace DNVGL.Veracity.Services.Api.This.Extensions
{
    public static class ConfigurationExtensions
    {

        public static IServiceCollection AddThisAdministrators(this IServiceCollection services, string clientConfigurationName = "administrators-this-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IThisAdministrators>(s => new ThisAdministrators(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddThisAdministrators(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IThisAdministrators>(s => new ThisAdministrators(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }
     
        public static IServiceCollection AddThisServices(this IServiceCollection services, string clientConfigurationName = "services-this-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IThisServices>(s => new ThisServices(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }


        public static IServiceCollection AddThisServices(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IThisServices>(s => new ThisServices(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddThisSubscribers(this IServiceCollection services, string clientConfigurationName = "subscribers-this-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IThisSubscribers>(s => new ThisSubscribers(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddThisSubscribers(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IThisSubscribers>(s => new ThisSubscribers(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }        

        public static IServiceCollection AddThisUsers(this IServiceCollection services, string clientConfigurationName = "users-this-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IThisUsers>(s => new ThisUsers(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }


        public static IServiceCollection AddThisUsers(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IThisUsers>(s => new ThisUsers(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

    }
}
