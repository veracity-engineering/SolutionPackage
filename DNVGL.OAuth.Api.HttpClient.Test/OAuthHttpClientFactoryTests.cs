using System;
using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DNVGL.OAuth.Api.HttpClient.Test
{
    public class OAuthHttpClientFactoryTests
    {
        [Test]
        public void CreateBothUserAndClientCredentialsHttpClients()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(context);

            var mockClientAppBuilder = new Mock<IClientAppBuilder>();

            var userHttpClientName = "UserFlow";
            var serverHttpClientName = "ServerFlow";

            var options = new List<OAuthHttpClientFactoryOptions> {
                new OAuthHttpClientFactoryOptions
                { 
                    BaseUri = "https://localhost/",
                    Name = userHttpClientName,
                    Flow = OAuthCredentialFlow.UserCredentials,
                    OAuthClientOptions = new OpenIdConnectOptions
                    {
                        Scopes = new []{ "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation" }
                    }
                },
                new OAuthHttpClientFactoryOptions
                { 
                    BaseUri = "https://veracity.com/",
                    Name = serverHttpClientName,
                    Flow = OAuthCredentialFlow.ClientCredentials,
                    OAuthClientOptions = new OpenIdConnectOptions
                    {
                        Scopes = new []{ "https://dnvglb2ctest.onmicrosoft.com/5d76a556-9394-48d4-8d11-786ddc3f54bc/.default" },
                        ClientId = Guid.NewGuid().ToString(),
                        ClientSecret = "none",
                        Authority = "https://logintest.veracity.com/tfp/ed815121-cdfa-4097-b524-e2b23cd36eb6/B2C_1A_SignInWithADFSIdp"
                    }
                }
            };
            var oauthHttpClientFactory = new OAuthHttpClientFactory(options, 
                mockHttpContextAccessor.Object, 
                mockClientAppBuilder.Object);

            var userHttpClient = oauthHttpClientFactory.Create(userHttpClientName);
            Assert.IsNotNull(userHttpClient);
            Assert.AreEqual(userHttpClient.BaseAddress.AbsoluteUri, "https://localhost/");

            var serverHttpClient = oauthHttpClientFactory.Create(serverHttpClientName);
            Assert.IsNotNull(serverHttpClient);
            Assert.AreEqual(serverHttpClient.BaseAddress.AbsoluteUri, "https://veracity.com/");
        }
    }
}