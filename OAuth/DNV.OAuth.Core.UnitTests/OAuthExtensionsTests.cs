using DNV.OAuth.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Xunit;

namespace DNV.OAuth.Core.UnitTests
{
	public class OAuthExtensionsTests
	{
		[Fact()]
		public void GetMsalAccountIdTest()
		{
			var tenantId = "a68572e3-63ce-4bc1-acdc-b64943502e9d";
			var issuer = $"https://login.veracity.com/{tenantId}/v2.0/";
			var objectId = new Claim(
				"http://schemas.microsoft.com/identity/claims/objectidentifier",
				Guid.NewGuid().ToString("d"),
				ClaimValueTypes.String,
				issuer
			);
			var policy = new Claim("http://schemas.microsoft.com/claims/authnclassreference", "signinsignup");
			var user = new ClaimsPrincipal(new ClaimsIdentity( new Claim[] { objectId, policy }));
			var accountId = user.GetMsalAccountId();
			Assert.Equal($"{objectId.Value}-{policy.Value}.{tenantId}", accountId);
		}

		[Fact()]
		public void AddOAuthCoreTest()
		{
			var services = new ServiceCollection();
			services.AddOAuthCore();
			var serviceProvider = services.BuildServiceProvider();
			var clientAppBuilder = serviceProvider.GetService<IClientAppBuilder>();
			Assert.IsType<MsalClientAppBuilder>(clientAppBuilder);
		}
	}
}