using DNVGL.OAuth.Api.HttpClient;
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
            var option = services.GetOauthClientOptions(clientConfigurationName);

            return services.AddApiV3<IMyCompanies>(s => new MyCompanies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));
        }


        public static IServiceCollection AddMyCompanies(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IMyCompanies>(s => new MyCompanies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }
             

        public static IServiceCollection AddMyMessages(this IServiceCollection services, string clientConfigurationName = "messages-my-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IMyMessages>(s => new MyMessages(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyMessages(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IMyMessages>(s => new MyMessages(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }


        public static IServiceCollection AddMyPolicies(this IServiceCollection services, string clientConfigurationName = "policies-my-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IMyPolicies>(s => new MyPolicies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyPolicies(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IMyPolicies>(s => new MyPolicies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyProfile(this IServiceCollection services, string clientConfigurationName = "profile-my-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IMyProfile>(s => new MyProfile(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyProfile(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IMyProfile>(s => new MyProfile(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyServices(this IServiceCollection services, string clientConfigurationName = "services-my-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IMyServices>(s => new MyServices(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyServices(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IMyServices>(s => new MyServices(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }
    }
}
