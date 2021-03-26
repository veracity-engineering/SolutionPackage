using System;

namespace DNVGL.OAuth.Api.HttpClient.Exceptions
{
	public class MissingClientConfigurationNameException: Exception
	{
		public MissingClientConfigurationNameException(string name) : base($"No instance of {nameof(OAuthHttpClientFactoryOptions)} could be retrieved where Name = '{name}'.") { }
	}
}
