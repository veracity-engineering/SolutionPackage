using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DNVGL.OAuth.Demo.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public async Task<IActionResult> Index()
		{
			return View();
		}

		public async Task<IActionResult> Signout()
		{
			await this.HttpContext.SignOutAsync();
			return this.RedirectToAction("Index");
		}
	}
}