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
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddThisAdministrators(option);
        }

        public static IServiceCollection AddThisAdministrators(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IThisAdministrators>(option
                                                , s => new ThisAdministrators(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

     
        public static IServiceCollection AddThisServices(this IServiceCollection services, string clientConfigurationName = "services-this-api")
        {
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddThisServices(option);
        }


        public static IServiceCollection AddThisServices(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IThisServices>(option
                                                , s => new ThisServices(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddThisSubscribers(this IServiceCollection services, string clientConfigurationName = "subscribers-this-api")
        {
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddThisSubscribers(option);
        }


        public static IServiceCollection AddThisSubscribers(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IThisSubscribers>(option
                                                , s => new ThisSubscribers(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        

        public static IServiceCollection AddThisUsers(this IServiceCollection services, string clientConfigurationName = "users-this-api")
        {
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddThisUsers(option);
        }


        public static IServiceCollection AddThisUsers(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IThisUsers>(option
                                                , s => new ThisUsers(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

    }
}
