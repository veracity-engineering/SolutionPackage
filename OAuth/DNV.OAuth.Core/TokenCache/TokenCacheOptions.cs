using Microsoft.Extensions.Caching.Distributed;
using System;

namespace DNV.OAuth.Core.TokenCache
{
	public class TokenCacheOptions : DistributedCacheEntryOptions
	{
		public TokenCacheOptions() : base()
		{
			this.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8);
		}
	}
}
