using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.TokenCacheProviders;
using System;

namespace DNVGL.OAuth.Demo.TokenCache
{
	public static class InMemoryTokenCacheProviderExtension
	{
		public static IServiceCollection AddInMemoryTokenCaches(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddMemoryCache();
			services.AddHttpContextAccessor();
			services.AddSingleton<IMsalTokenCacheProvider, MsalMemoryTokenCacheProvider>();
			return services;
		}

		public static AuthenticationBuilder AddInMemoryTokenCaches(this AuthenticationBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			builder.Services.AddInMemoryTokenCaches();
			return builder;
		}
	}
}
