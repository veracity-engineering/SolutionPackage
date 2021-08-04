using System;

namespace DNVGL.OAuth.Api.HttpClient
{
    public interface IOAuthHttpClientFactory
    {
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

    }
}
