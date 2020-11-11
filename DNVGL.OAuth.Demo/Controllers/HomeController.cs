using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo.Controllers
{
    [Authorize]
	public class HomeController : Controller
	{
		private IMsalClientApp _msalAppBuilder;

		public HomeController(IMsalClientApp msalAppBuilder)
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
			this.ViewBag.Account = account;
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