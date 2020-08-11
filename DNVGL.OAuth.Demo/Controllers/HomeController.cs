using DNVGL.OAuth.Demo.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private MsalAppBuilder _msalAppBuilder;

		public HomeController(MsalAppBuilder msalAppBuilder)
		{
			_msalAppBuilder = msalAppBuilder;
		}

		public async Task<IActionResult> Index()
		{
#if NETCORE2
			var version = "v2.1";
#elif NETCORE3
			var version = "v3.1";
#endif

			var account = await _msalAppBuilder.GetAccount(this.HttpContext);
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