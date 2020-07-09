using DNVGL.OAuth.UserCredentials;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Linq;
using System.Security.Claims;
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

        /*
        [HttpPost]
        [Route("/signin-oidc")]
        public IActionResult SignIn()
        {
            var form = HttpContext.Request.Form;
            return Json(form);
        }
        */

        [Route("/me")]
        public async Task<IActionResult> FetchUser()
        {
            return Json(await _userService.GetUser());
        }
    }
}
