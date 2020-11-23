using DNVGL.OAuth.Api.HttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.Directory.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddUserDirectory(this IServiceCollection services)
        {
            services.AddScoped<IUserDirectory>(s => new UserDirectory(s.GetRequiredService<IOAuthHttpClientFactory>()));
            return services;
        }

        public static IServiceCollection AddCompanyDirectory(this IServiceCollection services)
        {
            services.AddScoped<ICompanyDirectory>(s => new CompanyDirectory(s.GetRequiredService<IOAuthHttpClientFactory>()));
            return services;
        }
    }
}
