using System;
using System.Collections.Generic;

namespace DNVGL.OAuth.Api.HttpClient
{
	[Obsolete("Please use interface System.Net.Http.IHttpClientFactory in BCL instead.")]
    public interface IOAuthHttpClientFactory
    {
	    IEnumerable<OAuthHttpClientFactoryOptions> ClientOptions { get; }

        /// <summary>
        /// Creates an instance of <see cref="T:System.Net.Http.HttpClient"/> which will make authenticated requests according to
        /// the <see cref="OAuthHttpClientFactoryOptions"/> provided where the <see cref="OAuthHttpClientFactoryOptions.Name">Name</see> is matched by the argument.
        /// </summary>
        /// <param name="configPredict">an Func to predict which client configuration should be used.</param>
        /// <param name="configOverride">an Action which allow to override some configs for the client configuration.</param>
        /// <returns>Instance of <see cref="System.Net.Http.HttpClient"/> which can make authenticated requests.</returns>
        /// <exception cref="Exceptions.ClientConfigurationNotFoundException" />
        /// <exception cref="Exceptions.InvalidCredentialFlowException" />
        System.Net.Http.HttpClient Create(Func<OAuthHttpClientFactoryOptions, bool> configPredict, Action<OAuthHttpClientFactoryOptions> configOverride = null);

        /// <summary>
        /// Creates an instance of <see cref="T:System.Net.Http.HttpClient"/> which will make authenticated requests according to
        /// the <see cref="OAuthHttpClientFactoryOptions"/> provided where the <see cref="OAuthHttpClientFactoryOptions.Name">Name</see> is matched by the argument.
        /// </summary>
        /// <param name="apiName">The name of api setting in <see cref="OAuthHttpClientFactoryOptions"/></param>
        /// <returns>Instance of <see cref="System.Net.Http.HttpClient"/> which can make authenticated requests.</returns>
        /// <exception cref="Exceptions.ClientConfigurationNotFoundException" />
        /// <exception cref="Exceptions.InvalidCredentialFlowException" />
        [Obsolete("Please use additional 2 extension methods 'CreateWithUserCredentialFlow' or 'CreateWithClientCredentialFlow' instead.")]
        System.Net.Http.HttpClient Create(string apiName);
    }
}
