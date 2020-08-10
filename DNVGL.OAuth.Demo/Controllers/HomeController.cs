using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
			var identifier = "6144052f-7b8a-4c62-9f2e-259d2f37f39a-b2c_1a_signinwithadfsidp.a68572e3-63ce-4bc1-acdc-b64943502e9d";
			var account = await clientApp.GetAccountAsync(identifier);
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