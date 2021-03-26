using System;

namespace DNVGL.OAuth.Api.HttpClient.Exceptions
{
	[Serializable]
	public sealed class MissingClientConfigurationNameException: Exception
	{
		public MissingClientConfigurationNameException(string name) : base($"No instance of {nameof(OAuthHttpClientFactoryOptions)} could be retrieved where Name = '{name}'.") { }
	}
}
