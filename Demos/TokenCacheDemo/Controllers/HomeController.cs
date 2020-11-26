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
		private IMsalAppBuilder _msalAppBuilder;

		public HomeController(IMsalAppBuilder msalAppBuilder)
		{
			_msalAppBuilder = msalAppBuilder;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
				var account = await _msalAppBuilder.GetAccount(this.HttpContext);
				this.ViewBag.Account = account;
				var result = await _msalAppBuilder.AcquireTokenSilent(this.HttpContext);
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
