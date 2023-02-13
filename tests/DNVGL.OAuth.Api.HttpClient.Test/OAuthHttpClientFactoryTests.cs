using System;
using System.Collections.Generic;
using System.Net.Http;
using DNV.OAuth.Abstractions;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace DNVGL.OAuth.Api.HttpClient.Tests
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
                    OAuthClientOptions = new OAuth2Options
                    {
                        Scopes = new []{ "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation" }
                    }
                },
                new OAuthHttpClientFactoryOptions
                { 
                    BaseUri = "https://veracity.com/",
                    Name = serverHttpClientName,
                    Flow = OAuthCredentialFlow.ClientCredentials,
                    OAuthClientOptions = new OAuth2Options
                    {
                        Scopes = new []{ "https://dnvglb2ctest.onmicrosoft.com/5d76a556-9394-48d4-8d11-786ddc3f54bc/.default" },
                        ClientId = Guid.NewGuid().ToString(),
                        ClientSecret = "none",
                        Authority = "https://logintest.veracity.com/tfp/ed815121-cdfa-4097-b524-e2b23cd36eb6/B2C_1A_SignInWithADFSIdp"
                    }
                }
            };

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(f => 
		            f.CreateClient(It.Is<string>(name => name == $"{options[0].Name}:{options[0].Flow}")))
	            .Returns(() => new System.Net.Http.HttpClient { BaseAddress = new Uri(options[0].BaseUri) });
            mockFactory.Setup(f => 
		            f.CreateClient(It.Is<string>(name => name == $"{options[1].Name}:{options[1].Flow}")))
	            .Returns(() => new System.Net.Http.HttpClient { BaseAddress = new Uri(options[1].BaseUri) });

            var oauthHttpClientFactory = new OAuthHttpClientFactory(mockFactory.Object, options);

            var userHttpClient = oauthHttpClientFactory.CreateWithUserCredentialFlow(userHttpClientName);
            Assert.IsNotNull(userHttpClient);
            Assert.AreEqual(userHttpClient.BaseAddress.AbsoluteUri, "https://localhost/");

            var serverHttpClient = oauthHttpClientFactory.CreateWithClientCredentialFlow(serverHttpClientName);
            Assert.IsNotNull(serverHttpClient);
            Assert.AreEqual(serverHttpClient.BaseAddress.AbsoluteUri, "https://veracity.com/");
        }
    }
}