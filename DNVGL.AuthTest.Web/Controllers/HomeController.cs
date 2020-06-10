using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DNVGL.AuthTest.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("auth")]
        public async Task<IActionResult> Auth()
        {
            /*
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
    }
}
