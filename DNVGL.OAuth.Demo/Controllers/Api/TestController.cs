using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace DNVGL.SolutionPackage.Demo.Controllers.Api
{
	[ApiController]
	[Route("api/[controller]")]
	public class TestController : ControllerBase
	{
		[HttpGet("mobile")]
		[Authorize(AuthenticationSchemes = "ECOInsightMobileApi")]
		public IEnumerable<KeyValuePair<string, string>> GetMobileCliams()
		{
			return this.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
		}

		[HttpGet("janus")]
		[Authorize(AuthenticationSchemes = "JanusWeb")]
		public IEnumerable<KeyValuePair<string, string>> GetJanusCliams()
		{
			return this.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
		}
	}
}
