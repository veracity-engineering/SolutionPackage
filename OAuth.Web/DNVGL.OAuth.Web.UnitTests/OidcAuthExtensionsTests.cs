using Castle.Core.Configuration;
using DNV.OAuth.Abstractions;
using DNV.OAuth.Core.TokenCache;
using DNVGL.OAuth.Web.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;
using Microsoft.AspNetCore.Hosting;

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
				.AddLogging()
				.AddSingleton(CreateConfiguration());
			return services;
		}

		private static IConfigurationRoot CreateConfiguration()
		{
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
			return configuration;
		}

		private TestServer CreateServer()
		{
			var host = new HostBuilder()
				.ConfigureWebHost(b => b.UseTestServer()
					.ConfigureServices(services =>
					{
						var configuration = CreateConfiguration();
						b.ConfigureAppConfiguration((_, c) => c.AddConfiguration(configuration));
						var oidcOptions = new OidcOptions();
						configuration.GetSection("OidcAuthOptions").Bind(oidcOptions);

						services.AddOidc(oidcOptions);

						services.Configure<OpenIdConnectOptions>(o => {
							o.GetClaimsFromUserInfoEndpoint = true;
							o.Configuration = new OpenIdConnectConfiguration()
							{
								TokenEndpoint = "http://testhost/tokens",
								UserInfoEndpoint = "http://testhost/user",
								EndSessionEndpoint = "http://testhost/end"
							};
							o.StateDataFormat = new TestStateDataFormat();
							o.SecurityTokenValidator = new TestTokenValidator();
							o.ProtocolValidator = new TestProtocolValidator();
							o.BackchannelHttpHandler = new TestBackchannel();
						});
					})
					.Configure(app =>
					{
						app.UseAuthentication();
						app.Run(context => context.Response.WriteAsync(context.Request.Path));
					}))
				.Build();

			host.Start();
			return host.GetTestServer();
		}
	}

	internal class TestStateDataFormat : ISecureDataFormat<AuthenticationProperties>
	{
		public string Protect(AuthenticationProperties data) => "protected_state";

		public string Protect(AuthenticationProperties data, string purpose) => throw new NotImplementedException();

		public AuthenticationProperties Unprotect(string protectedText)
		{
			Assert.Equal("protected_state", protectedText);
			var items = new Dictionary<string, string?>()
			{
				{ ".xsrf", "correlationId" },
				{ OpenIdConnectDefaults.RedirectUriForCodePropertiesKey, "redirect_uri" },
				{ "testkey", "testvalue" }
			};
			var properties = new AuthenticationProperties(items)
			{
				RedirectUri = "http://testhost/redirect"
			};
			return properties;
		}

		public AuthenticationProperties Unprotect(string protectedText, string purpose) => throw new NotImplementedException();
	}

	internal class TestTokenValidator : ISecurityTokenValidator
	{
		public bool CanValidateToken => true;

		public int MaximumTokenSizeInBytes
		{
			get { return 1024; }
			set { throw new NotImplementedException(); }
		}

		public bool CanReadToken(string securityToken)
		{
			Assert.Equal("my_id_token", securityToken);
			return true;
		}

		public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
		{
			Assert.Equal("my_id_token", securityToken);
			validatedToken = new JwtSecurityToken();
			return new ClaimsPrincipal(new ClaimsIdentity("customAuthType"));
		}
	}

	internal class TestProtocolValidator : OpenIdConnectProtocolValidator
	{
		public override void ValidateAuthenticationResponse(OpenIdConnectProtocolValidationContext validationContext) { }
		public override void ValidateTokenResponse(OpenIdConnectProtocolValidationContext validationContext) { }
		public override void ValidateUserInfoResponse(OpenIdConnectProtocolValidationContext validationContext) { }
	}

	internal class TestBackchannel : HttpMessageHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (string.Equals("/tokens", request.RequestUri?.AbsolutePath, StringComparison.Ordinal))
			{
				return Task.FromResult(new HttpResponseMessage()
				{
					Content = new StringContent("{ \"id_token\": \"my_id_token\", \"access_token\": \"my_access_token\" }", Encoding.ASCII, "application/json")
				});
			}
			if (string.Equals("/user", request.RequestUri?.AbsolutePath, StringComparison.Ordinal))
			{
				return Task.FromResult(new HttpResponseMessage() { Content = new StringContent("{ }", Encoding.ASCII, "application/json") });
			}

			throw new NotImplementedException(request.RequestUri.ToString());
		}
	}
}