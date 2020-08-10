using System.Threading.Tasks;

namespace Microsoft.Identity.Client.TokenCacheProviders
{
	public interface IMsalTokenCacheProvider
	{
		Task InitializeAsync(ITokenCache tokenCache);
		Task ClearAsync(string homeAccountId);
	}
}

