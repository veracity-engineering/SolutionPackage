namespace DNVGL.OAuth.Api.HttpClient
{
    public interface IOAuthHttpClientFactory
    {
		/// <summary>
		/// Creates an instance of <see cref="T:System.Net.Http.HttpClient"/> which will make authenticated requests according to
		/// the <see cref="OAuthHttpClientFactoryOptions"/> provided where the <see cref="OAuthHttpClientFactoryOptions.Name">Name</see> is matched by the argument.
		/// </summary>
		/// <param name="name">Value to match <see cref="OAuthHttpClientFactoryOptions.Name">Name</see> of configuration options for the created instance.</param>
		/// <returns>Instance of <see cref="System.Net.Http.HttpClient"/> which can make authenticated requests.</returns>
		/// <exception cref="Exceptions.MissingClientConfigurationNameException" />
		/// <exception cref="Exceptions.InvalidCredentialFlowException" />
		System.Net.Http.HttpClient Create(string name);
    }
}
