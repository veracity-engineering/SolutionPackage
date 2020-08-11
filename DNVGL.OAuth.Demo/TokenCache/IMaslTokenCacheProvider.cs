using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo.TokenCache
{
	public interface IMsalTokenCacheProvider
	{
		Task InitializeAsync(ITokenCache tokenCache); 
		Task ClearAsync(string identifier);
	}
}

