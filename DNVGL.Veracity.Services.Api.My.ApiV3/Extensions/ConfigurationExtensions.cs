using DNVGL.OAuth.Api.HttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.My.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddUserDirectory(this IServiceCollection services)
        {
            services.AddScoped<IProfileMy>(s => new ProfileMy(s.GetRequiredService<IOAuthHttpClientFactory>()));
            return services;
        }
    }
}
