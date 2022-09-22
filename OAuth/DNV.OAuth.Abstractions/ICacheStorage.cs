using System.Threading.Tasks;

namespace DNV.OAuth.Abstractions
{
	public interface ICacheStorage
	{
		byte[]? Get(string key);

		Task<byte[]?> GetAsync(string key);

		void Set(string key, byte[]? value);

		Task SetAsync(string key, byte[]? value);

		void Remove(string key);

		Task RemoveAsync(string key);
	}
}
