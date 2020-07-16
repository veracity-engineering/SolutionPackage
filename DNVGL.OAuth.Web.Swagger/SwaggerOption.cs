using System.Collections.Generic;

namespace DNVGL.OAuth.Web.Swagger
{
	public class SwaggerOption
	{
		public bool Enabled { get; set; }
		public string Version { get; set; }
		public string Name { get; set; }

		public string AuthorizationUrl { get; set; }
		public Dictionary<string, string> Scopes { get; set; }
		public string ClientId { get; set; }
	}
}
