using DNVGL.OAuth.Api.HttpClient;
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
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddMyCompanies(option);
        }


        public static IServiceCollection AddMyCompanies(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IMyCompanies>(option
                                                , s => new MyCompanies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }
             

        public static IServiceCollection AddMyMessages(this IServiceCollection services, string clientConfigurationName = "messages-my-api")
        {
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddMyMessages(option);
        }

        public static IServiceCollection AddMyMessages(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IMyMessages>(option
                                                , s => new MyMessages(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }


        public static IServiceCollection AddMyPolicies(this IServiceCollection services, string clientConfigurationName = "policies-my-api")
        {
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddMyPolicies(option);
        }


        public static IServiceCollection AddMyPolicies(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IMyPolicies>(option
                                                , s => new MyPolicies(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyProfile(this IServiceCollection services, string clientConfigurationName = "profile-my-api")
        {
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddMyProfile(option);
        }

        public static IServiceCollection AddMyProfile(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IMyProfile>(option
                                                , s => new MyProfile(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddMyServices(this IServiceCollection services, string clientConfigurationName = "services-my-api")
        {
            var option = services.GetApiV3OauthClientOption(clientConfigurationName);

            return services.AddMyServices(option);
        }

        public static IServiceCollection AddMyServices(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddApiV3<IMyServices>(option
                                                , s => new MyServices(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }
    }
}
