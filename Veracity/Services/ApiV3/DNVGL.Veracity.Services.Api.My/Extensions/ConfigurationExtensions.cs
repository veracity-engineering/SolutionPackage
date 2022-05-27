﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DNVGL.Veracity.Services.Api.My.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddMyCompanies(this IServiceCollection services, string clientConfigurationName = "companies-my-api")
        {
            services.AddSerializer();
            services.AddSingleton<IMyCompanies>(s => new MyCompanies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetOauthClientOptions(clientConfigurationName)));

            return services;            
        } 

        public static IServiceCollection AddMyMessages(this IServiceCollection services, string clientConfigurationName = "messages-my-api")
        {
            services.AddSerializer();
            services.AddSingleton<IMyMessages>(s => new MyMessages(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetOauthClientOptions(clientConfigurationName)));         

            return services;
        }

        public static IServiceCollection AddMyPolicies(this IServiceCollection services, string clientConfigurationName = "policies-my-api")
        {
            services.AddSerializer();
            services.AddSingleton<IMyPolicies>(s => new MyPolicies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetOauthClientOptions(clientConfigurationName)));          

            return services;
        }  

        public static IServiceCollection AddMyProfile(this IServiceCollection services, string clientConfigurationName = "profile-my-api")
        {
            services.AddSerializer();
            services.AddSingleton<IMyProfile>(s => new MyProfile(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetOauthClientOptions(clientConfigurationName)));

            return services;
        }     

        public static IServiceCollection AddMyServices(this IServiceCollection services, string clientConfigurationName = "services-my-api")
        {
            services.AddSerializer();
            services.AddSingleton<IMyServices>(s => new MyServices(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetOauthClientOptions(clientConfigurationName)));         

            return services;
        }

        public static IServiceCollection AddMyWidgets(this IServiceCollection services, string clientConfigurationName = "widgets-my-api")
        {
            services.AddSerializer();
            services.AddSingleton<IMyWidgets>(s => new MyWidgets(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetOauthClientOptions(clientConfigurationName)));

            return services;
        }
    }
}
