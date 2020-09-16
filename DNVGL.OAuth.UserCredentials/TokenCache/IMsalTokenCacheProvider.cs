using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Api.HttpClient.TokenCache
{
    public interface IMsalTokenCacheProvider
	{
		Task InitializeAsync(ITokenCache tokenCache);
		Task ClearAsync(string identifier);
	}
}
