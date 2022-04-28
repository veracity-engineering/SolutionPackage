using DNVGL.OAuth.Api.HttpClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Veracity.Services.Api
{
	public class ApiV3OAuthHttpClientOptions
	{
		public IEnumerable<OAuthHttpClientOptions> Options { get; set; } = new List<OAuthHttpClientOptions>();
	}
}
