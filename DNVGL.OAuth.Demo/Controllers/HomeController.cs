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
		private IClientAppBuilder _clientApp;

		public HomeController(IClientAppBuilder clientApp)
		{
			_clientApp = clientApp;
		}

		public async Task<IActionResult> Index()
		{
#if NETCORE2
			var version = "v2.1";
#elif NETCORE3
			var version = "v3.1";
#endif

			var client = _clientApp.BuildForUserCredentials(this.HttpContext);
			var account = await client.GetAccount(this.HttpContext);
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