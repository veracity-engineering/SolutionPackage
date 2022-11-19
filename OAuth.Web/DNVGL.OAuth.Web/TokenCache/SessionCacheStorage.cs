using DNV.OAuth.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.TokenCache
{
	public class SessionCacheStorage : ICacheStorage
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private ISession Cache => _httpContextAccessor.HttpContext.Session;

		public SessionCacheStorage(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public byte[]? Get(string key) => this.Cache.Get(key);

		public Task<byte[]?> GetAsync(string key) => Task.FromResult(this.Get(key));

		public void Remove(string key) => this.Cache.Remove(key);

		public Task RemoveAsync(string key)
		{
			this.Remove(key);
			return Task.CompletedTask;
		}

		public void Set(string key, byte[]? value) => this.Cache.Set(key, value);

		public Task SetAsync(string key, byte[]? value)
		{
			this.Set(key, value);
			return Task.CompletedTask;
		}
	}
}
