using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Directory;
using DNVGL.Veracity.Services.Api.My;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.AuthTest.Web.Controllers
{
    public class ApiV3Controller : Controller
    {
        private readonly IUserDirectory _userDirectory;
        private readonly IMyProfile _myProfile;

        public ApiV3Controller(IUserDirectory userDirectory, IMyProfile myProfile)
        {
            _userDirectory = userDirectory;
            _myProfile = myProfile;
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

        [Authorize, Route("/api/profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _myProfile.Get();
            return Json(result);
        }
    }
}
