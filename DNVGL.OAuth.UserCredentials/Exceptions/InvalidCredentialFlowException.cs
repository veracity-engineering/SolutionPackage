using System;
using System.Runtime.Serialization;

namespace DNVGL.OAuth.Api.HttpClient.Exceptions
{
	[Serializable]
	public sealed class InvalidCredentialFlowException : Exception
	{
		public InvalidCredentialFlowException(OAuthCredentialFlow credentialFlow) : base($"Invalid credential flow '{credentialFlow}'.") { }

		private InvalidCredentialFlowException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
