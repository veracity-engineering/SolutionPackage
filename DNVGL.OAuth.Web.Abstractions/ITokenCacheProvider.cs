using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Web.Abstractions
{
    public interface ITokenCacheProvider
	{
		Task InitializeAsync(ITokenCache tokenCache);
		Task ClearAsync(string identifier);
	}
}
