using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.TokenCache;
using DNVGL.OAuth.Web.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DNVGL.OAuth.Web.UnitTests
{
	public class OidcAuthExtensionsTests
	{
		[Fact()]
		public void AddOidcTest()
		{
			var services = CreateServiceCollection();
			var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

			var oidcOptions = new OidcOptions();
			configuration.GetSection("OidcAuthOptions").Bind(oidcOptions);
			oidcOptions.Events = new OpenIdConnectEvents();

			services.AddOidc(oidcOptions);
			serviceProvider = services.BuildServiceProvider();

			var authenticationOptions = serviceProvider.GetService<IOptions<AuthenticationOptions>>()?.Value;
			Assert.NotNull(authenticationOptions);
			Assert.Equal(CookieAuthenticationDefaults.AuthenticationScheme, authenticationOptions.DefaultScheme);
			Assert.Equal(OpenIdConnectDefaults.AuthenticationScheme, authenticationOptions.DefaultChallengeScheme);

			var oauth2Options = serviceProvider.GetService<OAuth2Options>();
			Assert.NotNull(oauth2Options);
			Assert.True(oauth2Options.Scope?.StartsWith("https://dnvglb2cprod.onmicrosoft.com"));
			Assert.True(oauth2Options.Scope?.EndsWith("/user_impersonation"));

			var openIdConnectOptions = serviceProvider.GetService<IOptionsMonitor<OpenIdConnectOptions>>()?.Get(OpenIdConnectDefaults.AuthenticationScheme);
			Assert.NotNull(openIdConnectOptions);
			Assert.NotNull(openIdConnectOptions.Authority);
			Assert.Equal(oidcOptions.ClientId, openIdConnectOptions.ClientId);
			Assert.Equal(oidcOptions.ClientSecret, openIdConnectOptions.ClientSecret);
			Assert.Equal(oidcOptions.CallbackPath, openIdConnectOptions.CallbackPath);
			Assert.Equal(oidcOptions.ResponseType, openIdConnectOptions.ResponseType);
			Assert.True(openIdConnectOptions.UsePkce);
			Assert.Contains(oauth2Options.Scope, openIdConnectOptions.Scope);
			Assert.IsType<DNV.OAuth.Core.TokenValidator.DNVTokenValidator>(openIdConnectOptions.SecurityTokenValidator);
			Assert.Equal(oidcOptions.Events, openIdConnectOptions.Events);
		}

		[Fact()]
		public void AddOidcTest_Exceptions()
		{
			var services = CreateServiceCollection();
			var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

			Action<OidcOptions>? oidcSetupAction = null;
			AuthenticationBuilder func() => services.AddOidc(oidcSetupAction);

			Assert.Throws<ArgumentNullException>(func);
		}

		[Fact()]
		public void AddSessionTokenCachesTest()
		{
			var services = CreateServiceCollection();
			var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

			services.AddDistributedMemoryCache()
				.AddLogging();
			services.AddOidc(o => configuration.GetSection("OidcAuthOptions").Bind(o))
				.AddSessionTokenCaches();
			serviceProvider = services.BuildServiceProvider();

			var clientAppFactory = serviceProvider.GetService<IClientAppFactory>();
			var tokenCacheProvider = serviceProvider.GetService<ITokenCacheProvider>();
			var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
			var cache = serviceProvider.GetService<ISessionStore>();
			var cacheStorage = serviceProvider.GetService<ICacheStorage>();
			var sessionOptions = serviceProvider.GetService<IOptions<SessionOptions>>();


			Assert.NotNull(clientAppFactory);
			Assert.NotNull(tokenCacheProvider);
			Assert.NotNull(cache);
			Assert.NotNull(cacheStorage);
			Assert.IsType<SessionCacheStorage>(cacheStorage);
			Assert.Equal(true, sessionOptions?.Value?.Cookie.IsEssential);
		}

		[Fact()]
		public void AddInMemoryTokenCachesTest()
		{
			var slidingExpiration = TimeSpan.FromHours(8);

			var services = CreateServiceCollection();
			var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

			services.AddOidc(o => configuration.GetSection("OidcAuthOptions").Bind(o))
				.AddInMemoryTokenCaches(o => o.SetSlidingExpiration(slidingExpiration));
			serviceProvider = services.BuildServiceProvider();

			var clientAppFactory = serviceProvider.GetService<IClientAppFactory>();
			var tokenCacheProvider = serviceProvider.GetService<ITokenCacheProvider>();
			var cache = serviceProvider.GetService<IMemoryCache>();
			var cacheStorage = serviceProvider.GetService<ICacheStorage>();
			var memoryCacheEntryOptions = serviceProvider.GetService<IOptions<MemoryCacheEntryOptions>>();
			 
			Assert.NotNull(clientAppFactory);
			Assert.NotNull(tokenCacheProvider);
			Assert.NotNull(cache);
			Assert.NotNull(cacheStorage);
			Assert.IsType<MemoryCacheStorage>(cacheStorage);
			Assert.Equal(slidingExpiration, memoryCacheEntryOptions?.Value?.SlidingExpiration);
		}

		[Fact()]
		public void AddDistributedTokenCachesTest()
		{
			var slidingExpiration = TimeSpan.FromHours(8);

			var services = CreateServiceCollection();
			var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

			services.AddOidc(o => configuration.GetSection("OidcAuthOptions").Bind(o))
				.AddDistributedTokenCaches(o => o.SetSlidingExpiration(slidingExpiration));
			serviceProvider = services.BuildServiceProvider();

			var clientAppFactory = serviceProvider.GetService<IClientAppFactory>();
			var tokenCacheProvider = serviceProvider.GetService<ITokenCacheProvider>();
			var cache = serviceProvider.GetService<IDistributedCache>();
			var cacheStorage = serviceProvider.GetService<ICacheStorage>();
			var distributedCacheEntryOptions = serviceProvider.GetService<IOptions<DistributedCacheEntryOptions>>();

			Assert.NotNull(clientAppFactory);
			Assert.NotNull(tokenCacheProvider);
			Assert.NotNull(cache);
			Assert.NotNull(cacheStorage);
			Assert.IsType<DistributedCacheStorage>(cacheStorage);
			Assert.Equal(slidingExpiration, distributedCacheEntryOptions?.Value?.SlidingExpiration);
		}

		private static IServiceCollection CreateServiceCollection()
		{
			var services = new ServiceCollection()
				.AddLogging();

			var settings = new Dictionary<string, string>
			{
				{ "OidcAuthOptions:Environment", VeracityEnvironment.Production.ToString() },
				{ "OidcAuthOptions:ClientId", "34598bb3-b07f-4187-a32b-d64ef8f086bc" },
				{ "OidcAuthOptions:ClientSecret", "oN48Q~BQEiqjGBsdLotu00DWtOUsTBcLFOqDEbEd" },
				{ "OidcAuthOptions:SubscriptionKey", "3ea77e239dc84546971a84fc46035fb8" },
				{ "OidcAuthOptions:ResponseType", "code" },
			};
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(settings)
				.Build();
			services.AddSingleton(configuration);

			return services;
		}
	}
}