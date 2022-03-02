using System;
using System.Runtime.Serialization;

namespace DNVGL.OAuth.Api.HttpClient.Exceptions
{
	[Serializable]
	public sealed class ClientConfigurationNotFoundException : Exception
	{
		public ClientConfigurationNotFoundException() { }
		private ClientConfigurationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
