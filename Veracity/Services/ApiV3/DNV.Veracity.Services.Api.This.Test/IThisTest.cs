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

namespace DNV.Veracity.Services.Api.This.Tests
{
    [TestClass]
    public class IThisTest
    {
        ServiceCollection _services;        
        OAuthHttpClientFactoryOptions _options1;

        [TestInitialize]
        public void Init()
        { 
            _services = new ServiceCollection();

            _options1 = new OAuthHttpClientFactoryOptions()
            {
                Name = "test-config-name",
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
        public async Task AddThisUser_With_AddOAuthHttpClientFactory()
        {
            _services.AddDistributedMemoryCache();           

            _services.AddOAuthHttpClientFactory(new List<OAuthHttpClientFactoryOptions>()
            {               
                _options1,
                new OAuthHttpClientFactoryOptions(){
                     Name = "faketest",
                      Flow = OAuthCredentialFlow.ClientCredentials
                }
            });

            _services.AddThisUsers("test-config-name");

            var r = _services.BuildServiceProvider().GetRequiredService<IThisUsers>();
            var r2 = await r.Resolve("ming.ming.tim.tu@dnv.com");

            Assert.IsTrue(true);
        }


        [TestMethod]
        public async Task AddThisUser_With_AddOAuthHttpClients()
        {
            _services.AddDistributedMemoryCache();           

            _services.AddOAuthHttpClients(new List<OAuthHttpClientOptions>()
            {
                (OAuthHttpClientOptions)_options1,
                new OAuthHttpClientOptions(){
                     Name = "faketest",
                      Flow = OAuthCredentialFlow.ClientCredentials
                }
            });

            _services.AddThisUsers("test-config-name");

            var r = _services.BuildServiceProvider().GetRequiredService<IThisUsers>();
            var r2 = await r.Resolve("ming.ming.tim.tu@dnv.com");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task AddThisUser_With_AddOAuthHttpClients_WrongConfigName()
        {
            _services.AddDistributedMemoryCache();

            _services.AddOAuthHttpClients(new List<OAuthHttpClientOptions>()
            {
                (OAuthHttpClientOptions)_options1,
                new OAuthHttpClientOptions(){
                     Name = "faketest",
                      Flow = OAuthCredentialFlow.ClientCredentials
                }
            });

            _services.AddThisUsers("test-config-name:testfake");

            try
            {
                var r = _services.BuildServiceProvider().GetRequiredService<IThisUsers>();
                var r2 = await r.Resolve("ming.ming.tim.tu@dnv.com");
            }            
            catch (System.ArgumentException)
            {
                Assert.IsTrue(true);
            }              
        }

        [TestMethod]
        public async Task GetProfilePicture_Test()
        {
            _services.AddDistributedMemoryCache();
            _services.AddDataProtection();//for register IDataProtectionProvider

            _services.AddOAuthHttpClientFactory(new List<OAuthHttpClientFactoryOptions>()
            {
                _options1
            });

            _services.AddThisServices("test-config-name");

            var thisServices = _services.BuildServiceProvider().GetRequiredService<IThisServices>();

            var r = await thisServices.GetProfilePicture("8e80488d-9101-4dc7-a845-bff491147f47", "c94e947e-86fe-480f-9bee-ba32dced6136");

            Assert.IsTrue(true);
        }
    }
}