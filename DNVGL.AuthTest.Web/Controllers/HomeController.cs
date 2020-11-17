using DNVGL.OAuth.Api.HttpClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DNVGL.AuthTest.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return Ok(await _userService.GetUser());
        }

        [Route("user/{id}")]
        public async Task<IActionResult> UserById(string id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        [Route("/sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Json(new { message = "You signed out" });
        }
    }
}
