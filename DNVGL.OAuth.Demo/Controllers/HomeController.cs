using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private IDistributedCache cache;
		private IConfiguration config;

		public HomeController(IDistributedCache cache, IConfiguration config)
		{
			this.cache = cache;
			this.config = config;
		}

		public async Task<IActionResult> Index()
		{
#if NETCORE2
			var version = "v2.1";
#elif NETCORE3
			var version = "v3.1";
#endif

			var options = new OidcOptions();
			this.config.GetSection("Oidc").Bind(options);
			var clientApp = MsalAppBuilder.BuildConfidentialClientApplication(options, this.HttpContext);
			var tokenKey = this.User.FindFirst("TokenKey").Value;
			var account = await clientApp.GetAccountAsync(tokenKey);
			Debug.WriteLine(account);
			this.ViewBag.Version = version;
			return View();
		}

		public async Task<IActionResult> Signout()
		{
			await this.HttpContext.SignOutAsync();
			return this.RedirectToAction("Index");
		}
	}
}