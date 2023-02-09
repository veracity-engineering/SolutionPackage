using System;
using System.Runtime.Serialization;

namespace DNV.OAuth.Core.Exceptions
{
	[Serializable]
	public class MissingScopeException : Exception
	{
		public MissingScopeException() : base("Missing authentication scope in OAuth2 options, scope may not be null or empty.") { }

		private MissingScopeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
