using System.Net.Http;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
	public static class HttpClientFactoryExtensions
	{
		/// <summary>
		/// Creates and configures a <see cref="System.Net.Http.HttpClient"/> using configuration corresponding to the provided <paramref name="name"/> that is authenticated by the User Credential flow.
		/// </summary>
		/// <param name="factory">Http client factory available to create <see cref="System.Net.Http.HttpClient">HttpClient</see> instance.</param>
		/// <param name="name">Value to match <see cref="OAuthHttpClientOptions.Name">Name</see> of configuration options for the created instance.</param>
		/// <returns></returns>
		public static System.Net.Http.HttpClient CreateWithUserCredentialFlow(this IHttpClientFactory factory, string name) =>
			factory.CreateClient($"{name}:{OAuthCredentialFlow.UserCredentials}");

		/// <summary>
		/// Creates and configures a <see cref="System.Net.Http.HttpClient"/> using configuration corresponding to the provided <paramref name="name"/> that is authenticated by the Client Credential flow.
		/// </summary>
		/// <param name="factory">Http client factory available to create <see cref="System.Net.Http.HttpClient">HttpClient</see> instance.</param>
		/// <param name="name">Value to match <see cref="OAuthHttpClientOptions.Name">Name</see> of configuration options for the created instance.</param>
		/// <returns></returns>
		public static System.Net.Http.HttpClient CreateWithClientCredentialFlow(this IHttpClientFactory factory, string name) =>
			factory.CreateClient($"{name}:{OAuthCredentialFlow.ClientCredentials}");
	}
}
