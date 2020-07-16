using DNVGL.OAuth.Api.HttpClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DNVGL.AuthTest.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOAuthHttpClientFactory _httpClientFactory;
        private readonly UserService _userService;

        public HomeController(IOAuthHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _userService = new UserService(_httpClientFactory);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User.Identity;
            var identityUser = await _userService.GetUser();
            return Json(new { user, identityUser });
        }

        [Route("/me")]
        public async Task<IActionResult> FetchUser()
        {
            return Json(await _userService.GetUser());
        }

        [Route("/sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Json(new { message = "You signed out" });
        }
    }
}
