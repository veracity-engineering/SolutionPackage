using System.Linq;
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
        private const string ExpetctedDefaultCSP = "default-src 'self'; object-src 'self'; connect-src 'self' https://dc.services.visualstudio.com https://login.veracity.com https://login.microsoftonline.com; script-src 'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn; font-src 'self' data: https://onedesign.azureedge.net; media-src 'self'; worker-src 'self' blob:; img-src 'self' https://onedesign.azureedge.net; frame-src 'self' https://www.google.com https://www.recaptcha.net/; style-src 'self' https://onedesign.azureedge.net";
        private const string ExpetctedCustomizedCSP = "default-src 'self'; object-src 'self'; connect-src 'self' https://dc.services.visualstudio.com https://login.veracity.com https://login.microsoftonline.com; script-src 'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn; font-src 'self' data: https://onedesign.azureedge.net; media-src 'self'; worker-src 'self' blob:; img-src 'self' https://onedesign.azureedge.net; frame-src 'self' https://www.google.com https://www.recaptcha.net/; style-src 'self' 'nonce-123456789909876543ghjklkjvcvbnm'";

        [Fact]
        public async Task TestDefaultHeader()
        {

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseDefaultHeaders()
                    .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello World!", responseString);
            Assert.Equal("1", response.Headers.GetValues("X-Xss-Protection").FirstOrDefault());
            Assert.Equal("SAMEORIGIN", response.Headers.GetValues("X-Frame-Options").FirstOrDefault());
            Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").FirstOrDefault());
            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").FirstOrDefault());
            Assert.Equal("none", response.Headers.GetValues("X-Permitted-Cross-Domain-Policies").FirstOrDefault());
            Assert.Equal("enforce, max-age=7776000", response.Headers.GetValues("Expect-CT").FirstOrDefault());
            Assert.Equal("max-age=15552000; includeSubDomains", response.Headers.GetValues("Strict-Transport-Security").FirstOrDefault());
            Assert.Equal(ExpetctedDefaultCSP, response.Headers.GetValues("Content-Security-Policy").FirstOrDefault());
        }

        [Fact]
        public async Task TestCustomizedHeader()
        {

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseDefaultHeaders(h =>
                    {
                        h.Add("X-Xss-Protection", "0");
                        h.Add("X-Frame-Options", "DENNY");
                        h.Add("Referrer-Policy", "referrer");
                        h.Add("X-Content-Type-Options", "sniff");
                        h.Add("X-Permitted-Cross-Domain-Policies", "all");
                        h.Add("Expect-CT", "enforce, max-age=777");
                        h.Add("Strict-Transport-Security", "max-age=15552222; includeSubDomains");
                    })
                    .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            Assert.Equal("0", response.Headers.GetValues("X-Xss-Protection").FirstOrDefault());
            Assert.Equal("DENNY", response.Headers.GetValues("X-Frame-Options").FirstOrDefault());
            Assert.Equal("referrer", response.Headers.GetValues("Referrer-Policy").FirstOrDefault());
            Assert.Equal("sniff", response.Headers.GetValues("X-Content-Type-Options").FirstOrDefault());
            Assert.Equal("all", response.Headers.GetValues("X-Permitted-Cross-Domain-Policies").FirstOrDefault());
            Assert.Equal("enforce, max-age=777", response.Headers.GetValues("Expect-CT").FirstOrDefault());
            Assert.Equal("max-age=15552222; includeSubDomains", response.Headers.GetValues("Strict-Transport-Security").FirstOrDefault());
            Assert.Equal(ExpetctedDefaultCSP, response.Headers.GetValues("Content-Security-Policy").FirstOrDefault());
        }

        [Fact]
        public async Task TestKeepOriginalHeader()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.Use(async (context, next) =>
                        {
                            context.Response.Headers.Add("X-Xss-Protection", "1");
                        });
                        app.UseDefaultHeaders().Run(async ctx => await ctx.Response.WriteAsync("Hello World!"));
                    });
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            Assert.Equal("1", response.Headers.GetValues("X-Xss-Protection").FirstOrDefault());
        }

        [Fact]
        public async Task TestOverwwriteDefaultHeader()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.UseDefaultHeaders().Run(async ctx => await ctx.Response.WriteAsync("Hello World!"));
                        app.Use(async (context, next) =>
                        {
                            context.Response.Headers.Add("X-Xss-Protection", "1");
                        });
                    });
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            Assert.Equal("1", response.Headers.GetValues("X-Xss-Protection").FirstOrDefault());
        }

        [Fact]
        public async Task TestCustomizedCSPHeader()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseDefaultHeaders(h =>
                    {
                        h.Add("Content-Security-Policy", ExpetctedCustomizedCSP);
                        //h.ReplaceDefaultContentSecurityPolicy(styleSrc: "'self' 'nonce-123456789909876543ghjklkjvcvbnm'");
                    })
                    .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(ExpetctedCustomizedCSP, response.Headers.GetValues("Content-Security-Policy").FirstOrDefault());
        }

        [Fact]
        public async Task TestSkipCSPHeader()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseDefaultHeaders()
                    .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/Swagger/abc");
            response.EnsureSuccessStatusCode();

            Assert.False(response.Headers.Contains("Content-Security-Policy"));
        }

        [Fact]
        public async Task TestCustomizedSkipCSPHeader()
        {
            Thread.Sleep(3000);
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseDefaultHeaders(h=> {
                        h.ReplaceDefaultContentSecurityPolicy();
                        h.SkipContentSecurityPolicyOnRequests(t => t.Path.HasValue && t.Path.Value.Contains("skip"));
         
                    })
                    .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/test/skip");
            response.EnsureSuccessStatusCode();
            Assert.False(response.Headers.Contains("Content-Security-Policy"));

            response = await client.GetAsync("/hello");
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
        }

        [Fact]
        public async Task TestWebApiDefaultHeader()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseWebApiDefaultHeaders(skipRequest: req => req.Path.Value.Contains("swagger", System.StringComparison.InvariantCultureIgnoreCase))
                            .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            
            var response = await client.GetAsync("/swagger/webapi");
            response.EnsureSuccessStatusCode();
            Assert.False(response.Headers.Contains("Content-Security-Policy"));

            response = await client.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
            Assert.Equal("default-src 'none'", response.Headers.GetValues("Content-Security-Policy").FirstOrDefault());
        }

        
    }
}
