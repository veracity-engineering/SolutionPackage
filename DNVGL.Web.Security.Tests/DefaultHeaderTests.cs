using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace DNVGL.Web.Security.Tests
{
	public class DefaultHeaderTests
	{
		private static string Nonce;

		[Fact]
		public async Task TestDefaultHeader()
		{
			var (headers, responseString) = await MockResponse(app => app.UseDefaultHeaders());
			Assert.Equal("Hello World!", responseString);
			Assert.Equal("1", headers.GetValues("X-Xss-Protection").FirstOrDefault());
			Assert.Equal("SAMEORIGIN", headers.GetValues("X-Frame-Options").FirstOrDefault());
			Assert.Equal("no-referrer", headers.GetValues("Referrer-Policy").FirstOrDefault());
			Assert.Equal("nosniff", headers.GetValues("X-Content-Type-Options").FirstOrDefault());
			Assert.Equal("none", headers.GetValues("X-Permitted-Cross-Domain-Policies").FirstOrDefault());
			Assert.Equal("enforce, max-age=7776000", headers.GetValues("Expect-CT").FirstOrDefault());
			Assert.Equal("max-age=15552000; includeSubDomains", headers.GetValues("Strict-Transport-Security").FirstOrDefault());
			//Assert.Equal(ExpetctedDefaultCSP, headers.GetValues("Content-Security-Policy").FirstOrDefault());
			var csp = headers.GetValues("Content-Security-Policy").FirstOrDefault();
			Assert.Contains(Nonce, csp);
			Assert.Contains("connect-src: 'self' https://dc.services.visualstudio.com https://login.microsoftonline.com https://login.veracity.com https://loginstag.veracity.com https://logintest.veracity.com", csp);
			Assert.Contains($"style-src: 'self' https://onedesign.azureedge.net {Nonce}", csp);
			Assert.Contains(Nonce, csp);
		}

		[Fact]
		public async Task TestCustomizedHeader()
		{
			var (headers, _) = await MockResponse(app => app.UseDefaultHeaders(customizeHeaders: r =>
			{
				r.Set("X-Xss-Protection", "0");
				r.Set("X-Frame-Options", "DENNY");
				r.Set("Referrer-Policy", "referrer");
				r.Set("X-Content-Type-Options", "sniff");
				r.Set("X-Permitted-Cross-Domain-Policies", "all");
				r.Set("Expect-CT", "enforce, max-age=777");
				r.Set("Strict-Transport-Security", "max-age=15552222; includeSubDomains");
			}));

			Assert.Equal("0", headers.GetValues("X-Xss-Protection").FirstOrDefault());
			Assert.Equal("DENNY", headers.GetValues("X-Frame-Options").FirstOrDefault());
			Assert.Equal("referrer", headers.GetValues("Referrer-Policy").FirstOrDefault());
			Assert.Equal("sniff", headers.GetValues("X-Content-Type-Options").FirstOrDefault());
			Assert.Equal("all", headers.GetValues("X-Permitted-Cross-Domain-Policies").FirstOrDefault());
			Assert.Equal("enforce, max-age=777", headers.GetValues("Expect-CT").FirstOrDefault());
			Assert.Equal("max-age=15552222; includeSubDomains", headers.GetValues("Strict-Transport-Security").FirstOrDefault());
			//Assert.Equal(ExpetctedDefaultCSP, headers.GetValues("Content-Security-Policy").FirstOrDefault());
			Assert.Contains(Nonce, headers.GetValues("Content-Security-Policy").FirstOrDefault());
		}

		[Fact]
		public async Task TestSkipCSPHeader()
		{
			var (headers, _) = await MockResponse(
				app => app.UseDefaultHeaders(apiPredicate: request => request.Path.Value.Contains("/api/")),
				"/api/v1/customer"
			);

			Assert.Equal("frame-ancestors 'none'", headers.GetValues("Content-Security-Policy").FirstOrDefault());

			(headers, _) = await MockResponse(
				app => app.UseDefaultHeaders(exceptionPredicate: request => request.Path.Value.Contains("/swagger/")),
				"/swagger/index.html"
			);

			Assert.False(headers.Contains("Content-Security-Policy"));
		}

		private static async Task<(HttpResponseHeaders, string)> MockResponse(Action<IApplicationBuilder> configure, string uri = "/")
		{
			var hostBuilder = new HostBuilder()
				.ConfigureWebHost(webHost =>
				{
					webHost.UseTestServer();
					webHost.Configure(app =>
					{
						configure(app);
						app.Run(async ctx =>
						{
							Nonce = ctx.CreateNonce();
							await ctx.Response.WriteAsync("Hello World!");
						});
					});
				});

			var host = await hostBuilder.StartAsync();
			var client = host.GetTestClient();
			var response = await client.GetAsync(uri);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			return (response.Headers, responseString);
		}
	}
}
