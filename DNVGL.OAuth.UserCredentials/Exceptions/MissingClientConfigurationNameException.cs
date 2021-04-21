using System;
using System.Runtime.Serialization;

namespace DNVGL.OAuth.Api.HttpClient.Exceptions
{
	[Serializable]
	public sealed class MissingClientConfigurationNameException : Exception
	{
		public MissingClientConfigurationNameException(string name) : base($"No instance of {nameof(OAuthHttpClientFactoryOptions)} could be retrieved where Name = '{name}'.") { }

		private MissingClientConfigurationNameException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
