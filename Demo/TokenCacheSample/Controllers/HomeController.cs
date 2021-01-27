using DNVGL.OAuth.Web.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace TokenCacheDemo.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private IClientAppBuilder _appBuilder;

		public HomeController(IClientAppBuilder appBuilder)
		{
			_appBuilder = appBuilder;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
				var clientApp = _appBuilder.BuildForUserCredentials(this.HttpContext);
				var account = await clientApp.GetAccount(this.HttpContext);
				this.ViewBag.Account = account;
				var result = await clientApp.AcquireTokenSilent(this.HttpContext);
				this.ViewBag.Token = result.AccessToken ?? result.IdToken;
			}
			catch(Exception e)
			{
			}

			return View();
		}

		public async Task<IActionResult> Signout()
		{
			await this.HttpContext.SignOutAsync();
			return this.RedirectToAction("Index");
		}
	}
}
