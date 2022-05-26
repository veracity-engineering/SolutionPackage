using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Directory.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using DNVGL.OAuth.Api.HttpClient.Extensions;

namespace DNVGL.Veracity.Services.Api.Directory.Extensions
{
    public static class ConfigurationExtensions
    {
		public static IServiceCollection AddCompanyDirectory(this IServiceCollection services, string clientConfigurationName = "company-directory-api")
		{
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<ICompanyDirectory>(s => new CompanyDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
		}

		public static IServiceCollection AddCompanyDirectory(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<ICompanyDirectory>(s => new CompanyDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }


        public static IServiceCollection AddServiceDirectory(this IServiceCollection services, string clientConfigurationName = "service-directory-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IServiceDirectory>(s => new ServiceDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }

        public static IServiceCollection AddServiceDirectory(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IServiceDirectory>(s => new ServiceDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }


        public static IServiceCollection AddUserDirectory(this IServiceCollection services, string clientConfigurationName = "user-directory-api")
        {
            var option = services.GetOauthClientOptions(clientConfigurationName);

            services.AddApiV3<IUserDirectory>(s => new UserDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;    
        }

        public static IServiceCollection AddUserDirectory(this IServiceCollection services, OAuthHttpClientOptions option)
        {
            services.AddOAuthHttpClient(option);
            services.AddApiV3<IUserDirectory>(s => new UserDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), option));

            return services;
        }
    }
}
