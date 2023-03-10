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
            services.AddSerializer();  
            services.AddSingleton<IThisAdministrators>(s => new ThisAdministrators(new ApiClientFactory(s.GetAllOAuthClientOptions(clientConfigurationName), s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>())));

            return services;
        }       
     
        public static IServiceCollection AddThisServices(this IServiceCollection services, string clientConfigurationName = "services-this-api")
        {
            services.AddSerializer();  
            services.AddSingleton<IThisServices>(s => new ThisServices(new ApiClientFactory(s.GetAllOAuthClientOptions(clientConfigurationName), s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>())));

            return services;
        }

        public static IServiceCollection AddThisSubscribers(this IServiceCollection services, string clientConfigurationName = "subscribers-this-api")
        {
            services.AddSerializer();                      
            services.AddSingleton<IThisSubscribers>(s => new ThisSubscribers(new ApiClientFactory(s.GetAllOAuthClientOptions(clientConfigurationName), s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>())));

            return services;
        }

        public static IServiceCollection AddThisUsers(this IServiceCollection services, string clientConfigurationName = "users-this-api")
        {
            services.AddSerializer();
            services.AddSingleton<IThisUsers>(s => new ThisUsers(new ApiClientFactory(s.GetAllOAuthClientOptions(clientConfigurationName), s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>())));

            return services;
        }
    }
}
