using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNVGL.AuthTest.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var user = HttpContext.User.Identity;
            var signedInUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Json(new { signedInUserId, user });
        }

        [Authorize]
        [Route("auth")]
        public IActionResult Auth()
        {
            /*
            var user = HttpContext.User;
            var scope = new[] { "https://dnvglb2ctest.onmicrosoft.com/efb3e529-2f80-458b-aedf-7f4c8c794b45" };// AzureAdB2COptions.ApiScopes.Split(' ');
            var signedInUserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IConfidentialClientApplication cca =
            ConfidentialClientApplicationBuilder.Create("6f0bb6fa-e604-43cd-9414-42def1ac7deb")
                .WithRedirectUri("")
                .WithClientSecret("g.i1k-B_63p-oi5U6oQSL5V0DVY2iGZXJ~")
                .WithAuthority("https://login.microsoftonline.com/dnvglb2ctest.onmicrosoft.com")
                .Build();
            //new MSALStaticCache(signedInUserID, this.HttpContext).EnablePersistence(cca.UserTokenCache);

            var accounts = await cca.GetAccountsAsync();
            AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();

            return Json(result);
            */
            return Json(new { message = "Hello world" });
        }

        [HttpPost]
        [Route("/signin-oidc")]
        public IActionResult SignIn()
        {
            var form = HttpContext.Request.Form;
            return Json(form);
        }

        [Route("arrive")]
        public IActionResult Arrive()
        {
            var signedIn = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return Json(new { message = "You have arrived!" });
        }
    }
}
