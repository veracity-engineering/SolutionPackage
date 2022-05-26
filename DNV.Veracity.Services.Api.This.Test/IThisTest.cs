using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.This.Abstractions;
using DNVGL.Veracity.Services.Api.This.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.OAuth.Api.HttpClient.Extensions;

namespace DNV.Veracity.Services.Api.This.Test
{
    [TestClass]
    public class IThisTest
    {
        ServiceCollection _services;
        OAuthHttpClientOptions _options1;
        OAuthHttpClientFactoryOptions _options2;

        [TestInitialize]
        public void Init()
        { 
            _services = new ServiceCollection();

            _options1 = new OAuthHttpClientOptions()
            {
                Name = "test",
                Flow = OAuthCredentialFlow.ClientCredentials,
                BaseUri = "https://api-test.veracity.com",
                SubscriptionKey = "8974ad4960b34d2a825653311a8b8b1f",
                OAuthClientOptions = new OAuth.Abstractions.OAuth2Options()
                {
                    Authority = "https://login.microsoftonline.com/dnvglb2ctest.onmicrosoft.com/v2.0",
                    Scopes = new[] { "https://dnvglb2ctest.onmicrosoft.com/efb3e529-2f80-458b-aedf-7f4c8c794b45/.default" },
                    ClientId = "6f0bb6fa-e604-43cd-9414-42def1ac7deb",
                    ClientSecret = "8PXnw5bo.aQ95GOKs-RyErokg_ooifcCK."
                }
            };

            _options2 = new OAuthHttpClientFactoryOptions()
            {
                Name = "test",
                Flow = OAuthCredentialFlow.ClientCredentials,
                BaseUri = "https://api-test.veracity.com",
                SubscriptionKey = "8974ad4960b34d2a825653311a8b8b1f",
                OAuthClientOptions = new OAuth.Abstractions.OAuth2Options()
                {
                    Authority = "https://login.microsoftonline.com/dnvglb2ctest.onmicrosoft.com/v2.0",
                    Scopes = new[] { "https://dnvglb2ctest.onmicrosoft.com/efb3e529-2f80-458b-aedf-7f4c8c794b45/.default" },
                    ClientId = "6f0bb6fa-e604-43cd-9414-42def1ac7deb",
                    ClientSecret = "8PXnw5bo.aQ95GOKs-RyErokg_ooifcCK."
                }
            };
        }             

        [TestMethod]
        public async Task AddThisUserWithOptionTest()
        {
            _services.AddDistributedMemoryCache();

            _services.AddThisUsers(_options1);

            var r = _services.BuildServiceProvider().GetRequiredService<IThisUsers>();
            var r2 = await r.Resolve("ming.ming.tim.tu@dnv.com");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task AddThisUserWithName_OldOptionsType()
        {
            _services.AddDistributedMemoryCache();

            _services.AddOAuthHttpClientFactory(new List<OAuthHttpClientFactoryOptions>()
            {
                _options2,
                new OAuthHttpClientFactoryOptions(){
                     Name = "test2"
                }
            });

            _services.AddThisUsers("test");

            var r = _services.BuildServiceProvider().GetRequiredService<IThisUsers>();
            var r2 = await r.Resolve("ming.ming.tim.tu@dnv.com");

            Assert.IsTrue(true);
        }
    }
}