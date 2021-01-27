using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.Directory.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddCompanyDirectory(this IServiceCollection services, string clientConfigurationName = "company-directory-api")
        {
            services.AddSerializer();
            services.AddSingleton<ICompanyDirectory>(s => new CompanyDirectory(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>(), clientConfigurationName));
            return services;
        }

        public static IServiceCollection AddServiceDirectory(this IServiceCollection services, string clientConfigurationName = "service-directory-api")
        {
            services.AddSerializer();
            services.AddSingleton<IServiceDirectory>(s => new ServiceDirectory(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>(), clientConfigurationName));
            return services;
        }

        public static IServiceCollection AddUserDirectory(this IServiceCollection services, string clientConfigurationName = "user-directory-api")
        {
            services.AddSerializer();
            services.AddSingleton<IUserDirectory>(s => new UserDirectory(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>(), clientConfigurationName));
            return services;
        }
    }
}
