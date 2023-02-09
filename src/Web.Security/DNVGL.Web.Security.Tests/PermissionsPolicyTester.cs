using System;
using System.Linq;
using System.Threading.Tasks;
using DNVGL.Web.Security.PermissionsPolicies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace DNVGL.Web.Security.Tests
{
    public class PermissionsPolicyTester
    { 
        [Fact]
        public void Test_Single_Feature()
        {
            var policy = new PermissionsPolicy();
            var feature1 = policy.Feature("accelerometer");
            feature1.Disable();
            Assert.Equal("accelerometer=()", policy.ToString());
            var builder1 = feature1.Enable();
            Assert.Equal("accelerometer=*", policy.ToString());
            builder1.Self();
            Assert.Equal("accelerometer=(self)", policy.ToString());
            builder1.Custom("https://www.dnv.com");
            var policy1Str = policy.ToString().Replace("accelerometer=", "");
            var len = policy1Str.Length;
            var contentArr = policy1Str.Substring(1, len - 2).Split(",");
            Assert.True(Array.IndexOf(contentArr, "self") > -1);
            Assert.True(Array.IndexOf(contentArr, "\"https://www.dnv.com\"") > -1);
        }

        [Fact]
        public void Test_Multiple_Features()
        {
            var policy = new PermissionsPolicy();
            policy.Feature(FeatureNames.Camera).Disable();
            policy.Feature(FeatureNames.Fullscreen).Enable();
            policy.Feature(FeatureNames.Geolocation).Enable().Self();
            policy.Feature(FeatureNames.Usb).Enable().Custom("https://www.dnv.com");
            policy.Feature(FeatureNames.Microphone).Enable().Self().Custom("https://www.google.com");

            var policyStr = policy.ToString();
            Assert.Contains($"{FeatureNames.Camera}=()", policyStr);
            Assert.Contains($"{FeatureNames.Fullscreen}=*", policyStr);
            Assert.Contains($"{FeatureNames.Geolocation}=(self)", policyStr);
            Assert.Contains($"{FeatureNames.Usb}=(\"https://www.dnv.com\")", policyStr);
            Assert.Contains($"{FeatureNames.Microphone}=(self,\"https://www.google.com\")", policyStr);
        }

        [Fact]
        public async Task Test_Disable_All_Features()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseDefaultSecurityHeaders(customizeHeaders: h => h.DisableAllPermissionsPolicy())
                    .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/default");
            response.EnsureSuccessStatusCode();

            FeatureNames.All.ForEach(name => {
                Assert.Contains($"{name}=()", response.Headers.GetValues(PermissionsPolicy.Key).FirstOrDefault());
            });
            
        }

        [Fact]
        public async Task Test_Enable_All_Features_For_Self()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.UseDefaultSecurityHeaders(customizeHeaders: h => h.EnableAllPermissionsPolicyForSelf())
                    .Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();
            var response = await client.GetAsync("/default");
            response.EnsureSuccessStatusCode();

            FeatureNames.All.ForEach(name => {
                Assert.Contains($"{name}=(self)", response.Headers.GetValues(PermissionsPolicy.Key).FirstOrDefault());
            });

        }
    }
}
