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
