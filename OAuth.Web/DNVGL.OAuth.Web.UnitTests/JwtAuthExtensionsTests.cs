using DNV.OAuth.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace DNVGL.OAuth.Web.UnitTests
{
	public class JwtAuthExtensionsTests
	{
		[Fact()]
		public void AddJwtTest_Single()
		{
			var services = CreateServiceCollection();
			var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

			Action<JwtOptions> configureOptions = null;

			AuthenticationBuilder action() => services.AddJwtDefault()
				.AddJwt("ClientA", configureOptions);
			Assert.Throws<ArgumentNullException>(action);

			configureOptions = o =>
			{
				configuration?.GetSection("JwtAuthOptions:ClientA").Bind(o);
				o.TokenValidationParameters = new TokenValidationParameters();
				o.Events = new JwtBearerEvents();
			};
			var jwtOptions = new JwtOptions();
			configureOptions(jwtOptions);

			services.AddJwtDefault()
				.AddJwt("ClientA", configureOptions);
			serviceProvider = services.BuildServiceProvider();

			var authenticationOptions = serviceProvider.GetService<IOptions<AuthenticationOptions>>()?.Value;
			Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authenticationOptions?.DefaultScheme);

			var jwtBearerOptions = serviceProvider.GetService<IOptionsMonitor<JwtBearerOptions>>()?.Get("ClientA");

			Assert.NotNull(jwtBearerOptions);
			Assert.Equal(jwtOptions.Authority, jwtBearerOptions.Authority);
			Assert.Equal(jwtOptions.ClientId, jwtBearerOptions.Audience);
			Assert.NotEmpty(jwtBearerOptions.SecurityTokenValidators);
			Assert.IsType<DNV.OAuth.Core.TokenValidator.DNVTokenValidator>(jwtBearerOptions.SecurityTokenValidators.First());
		}

		[Fact()]
		public void AddJwtTest_Multiple()
		{
			var services = CreateServiceCollection();
			var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

			AuthenticationBuilder action1() => services.AddJwtDefault()
				.AddJwt(configuration.GetSection("NotExistOptions")?.GetChildren());
			Assert.Throws<ArgumentNullException>(action1);

			AuthenticationBuilder action2() => services.AddJwtDefault()
				.AddJwt(new Dictionary<string, JwtOptions>());
			Assert.Throws<ArgumentNullException>(action2);

			services.AddJwtDefault()
				.AddJwt(configuration.GetSection("JwtAuthOptions").GetChildren());
			serviceProvider = services.BuildServiceProvider();

			var authenticationOptions = serviceProvider.GetService<IOptions<AuthenticationOptions>>()?.Value;
			Assert.NotNull(authenticationOptions);
			Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authenticationOptions.DefaultScheme);
			var schemes = authenticationOptions.Schemes.Select(s => s.Name);
			Assert.Contains(schemes, s => s == "ClientA");
			Assert.Contains(schemes, s => s == "ClientB");
		}

		private static IServiceCollection CreateServiceCollection()
		{
			var services = new ServiceCollection()
				.AddLogging();
			var veracityOptions = VeracityOptions.Get(VeracityEnvironment.Production);

			var settings = new Dictionary<string, string>
			{
				{ "JwtAuthOptions:ClientA:Authority", veracityOptions.AADAuthorityV2 },
				{ "JwtAuthOptions:ClientA:ClientId", "34598bb3-b07f-4187-a32b-d64ef8f086bc" },
				{ "JwtAuthOptions:ClientB:Authority", veracityOptions.AADAuthorityV2 },
				{ "JwtAuthOptions:ClientB:ClientId", "6c649150-3662-4a1c-a653-f99e9398185f" },
			};
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(settings)
				.Build();
			services.AddSingleton(configuration);

			return services;
		}
	}
}