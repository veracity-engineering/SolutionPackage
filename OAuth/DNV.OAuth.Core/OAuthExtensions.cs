using System;
using System.Security.Claims;
using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.TokenCache;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNV.OAuth.Core
{
	public static class OAuthExtensions
	{
		private const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
		private const string Policy = "http://schemas.microsoft.com/claims/authnclassreference";

		/// <summary>
		/// Generates a MSAL Account Id from user claims and OIDC Options.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		internal static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal)
		{
			var objectId = claimsPrincipal.FindFirst(ObjectId);
			var policy = claimsPrincipal.FindFirst(Policy)?.Value;
			var tenantId = objectId.Issuer.Split('/')[3];
			var msalAccountId = $"{objectId.Value}-{policy}.{tenantId}";
			return msalAccountId.ToLower();
		}

		/// <summary>
		/// Register OAuthCore required services to DI container.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configAction"></param>
		public static IServiceCollection AddOAuthCore(this IServiceCollection services, Action<DistributedCacheEntryOptions>? configAction = null)
		{
			if (configAction != null)
				services.Configure(nameof(TokenCacheProvider), configAction);
			
            services.TryAddSingleton<ITokenCacheProvider, TokenCacheProvider>();

            services.TryAddSingleton<IClientAppBuilder, MsalClientAppBuilder>();

			return services;
		}
	}
}
