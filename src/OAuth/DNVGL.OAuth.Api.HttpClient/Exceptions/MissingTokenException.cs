using System;
using System.Runtime.Serialization;

namespace DNVGL.OAuth.Api.HttpClient.Exceptions
{
	[Serializable]
	public class MissingTokenException : Exception
	{
		public MissingTokenException() : base("Authentication token was null or empty.") { }

		private MissingTokenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
