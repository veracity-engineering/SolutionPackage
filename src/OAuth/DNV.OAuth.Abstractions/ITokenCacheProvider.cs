using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace DNV.OAuth.Abstractions
{
	public interface ITokenCacheProvider
	{
		Task InitializeAsync(ITokenCache tokenCache);
		Task ClearAsync(string identifier);
	}
}
