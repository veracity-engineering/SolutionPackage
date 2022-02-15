using System;
using System.Runtime.Serialization;

namespace DNVGL.OAuth.Web.Exceptions
{
	[Serializable]
	public sealed class TokenExpiredException: Exception
	{
		public TokenExpiredException() { }
		private TokenExpiredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
