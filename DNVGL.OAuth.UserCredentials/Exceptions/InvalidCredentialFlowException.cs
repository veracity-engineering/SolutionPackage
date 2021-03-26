using System;

namespace DNVGL.OAuth.Api.HttpClient.Exceptions
{
	[Serializable]
	public sealed class InvalidCredentialFlowException : Exception
	{
		public InvalidCredentialFlowException(OAuthCredentialFlow credentialFlow) : base($"Invalid credential flow '{credentialFlow}'.") { }
	}
}
