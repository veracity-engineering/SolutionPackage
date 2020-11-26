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
                    Flow = OAuthCredentialFlow.UserCredentials
                },
                new OAuthHttpClientFactoryOptions
                { 
                    BaseUri = "https://veracity.com/",
                    Name = serverHttpClientName,
                    Flow = OAuthCredentialFlow.ClientCredentials
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