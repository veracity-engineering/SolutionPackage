using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Directory;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.AuthTest.Web.Controllers
{
    public class ApiV3Controller : Controller
    {
        private readonly IUserDirectory _userDirectory;

        public ApiV3Controller(IUserDirectory userDirectory)
        {
            _userDirectory = userDirectory;
        }

        [Route("/api/users/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await _userDirectory.Get(userId);
            return Json(result);
        }

        [Route("/api/services/{userId}")]
        public async Task<IActionResult> GetUserServices(string userId)
        {
            var result = await _userDirectory.ListServices(userId);
            return Json(result);
        }

        [Route("/api/companies/{userId}")]
        public async Task<IActionResult> GetUserCompanies(string userId)
        {
            var result = await _userDirectory.ListCompanies(userId);
            return Json(result);
        }
    }
}
