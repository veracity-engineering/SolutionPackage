using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace DNV.OAuth.Abstractions
{
	public interface ITokenCacheProvider
	{
		Task InitializeAsync(ITokenCache tokenCache);
		Task ClearAsync(string identifier);
	}
}
