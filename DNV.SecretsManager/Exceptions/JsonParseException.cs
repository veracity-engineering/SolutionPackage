using System;
using System.Runtime.Serialization;

namespace DNV.SecretsManager.Exceptions
{
	[Serializable]
	public sealed class JsonParseException : Exception
	{
		public JsonParseException(string pathSegment) : base($"Unable to parse array index: {pathSegment}") { }
		private JsonParseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
