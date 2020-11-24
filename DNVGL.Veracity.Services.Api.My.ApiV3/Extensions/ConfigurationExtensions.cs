using DNVGL.OAuth.Api.HttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.My.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddMyProfile(this IServiceCollection services)
        {
            services.AddScoped<IMyProfile>(s => new MyProfile(s.GetRequiredService<IOAuthHttpClientFactory>()));
            return services;
        }
    }
}
