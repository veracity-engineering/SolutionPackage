using System;
using System.Runtime.Serialization;

namespace DNV.OAuth.Core.Exceptions
{
	[Serializable]
	public class MissingScopesException : Exception
	{
		public MissingScopesException() : base("Missing authentication scope, scope should not be null or empty.") { }

		private MissingScopesException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
