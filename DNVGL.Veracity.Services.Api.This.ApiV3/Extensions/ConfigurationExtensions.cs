using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.This.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddThisAdministrators(this IServiceCollection services, string clientConfigurationName = "administrators-this-api")
        {
            services.AddSerializer();
            services.AddSingleton<IThisAdministrators>(s => new ThisAdministrators(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>(), clientConfigurationName));
            return services;
        }

        public static IServiceCollection AddThisServices(this IServiceCollection services, string clientConfigurationName = "services-this-api")
        {
            services.AddSerializer();
            services.AddSingleton<IThisServices>(s => new ThisServices(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>(), clientConfigurationName));
            return services;
        }

        public static IServiceCollection AddThisSubscribers(this IServiceCollection services, string clientConfigurationName = "subscribers-this-api")
        {
            services.AddSerializer();
            services.AddSingleton<IThisServices>(s => new ThisServices(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>(), clientConfigurationName));
            return services;
        }

        public static IServiceCollection AddThisUsers(this IServiceCollection services, string clientConfigurationName = "users-this-api")
        {
            services.AddSerializer();
            services.AddSingleton<IThisUsers>(s => new ThisUsers(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>(), clientConfigurationName));
            return services;
        }
    }
}
