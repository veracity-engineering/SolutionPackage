using System;
using System.Runtime.Serialization;

namespace DNV.OAuth.Core.Exceptions
{
	[Serializable]
	public sealed class TokenExpiredException: Exception
	{
		public TokenExpiredException() { }

		private TokenExpiredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
