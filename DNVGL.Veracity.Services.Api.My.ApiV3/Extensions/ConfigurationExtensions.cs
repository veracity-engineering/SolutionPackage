using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.My.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddMyProfile(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer>(s => new JsonSerializer());
            services.AddSingleton<IMyProfile>(s => new MyProfile(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>()));
            return services;
        }

        public static IServiceCollection AddMyCompanies(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer>(s => new JsonSerializer());
            services.AddSingleton<IMyCompanies>(s => new MyCompanies(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>()));
            return services;
        }
    }
}
