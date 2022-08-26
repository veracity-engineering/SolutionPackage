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
            services.AddSerializer();          
            services.AddSingleton<ICompanyDirectory>(s => new CompanyDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetAllOAuthClientOptions(clientConfigurationName)));

            return services;
		}	

        public static IServiceCollection AddServiceDirectory(this IServiceCollection services, string clientConfigurationName = "service-directory-api")
        {
            services.AddSerializer();          
            services.AddSingleton<IServiceDirectory>(s => new ServiceDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetAllOAuthClientOptions(clientConfigurationName)));
         
            return services;
        }

        public static IServiceCollection AddUserDirectory(this IServiceCollection services, string clientConfigurationName = "user-directory-api")
        {
            services.AddSerializer();           
            services.AddSingleton<IUserDirectory>(s => new UserDirectory(s.GetRequiredService<IHttpClientFactory>(), s.GetRequiredService<ISerializer>(), s.GetAllOAuthClientOptions(clientConfigurationName)));
           
            return services;    
        }        
    }
}
