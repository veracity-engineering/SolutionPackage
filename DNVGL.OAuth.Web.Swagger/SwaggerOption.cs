using System.Collections.Generic;

namespace DNVGL.OAuth.Web.Swagger
{
	public class SwaggerOption
	{
		public bool Enabled { get; set; }
		public string Version { get; set; }
		public string Name { get; set; }
		public bool AuthencitationRequired { get; set; }
		public string AuthorizationEndpoint { get; set; }
		public string TokenEndpoint { get; set; }
		public IEnumerable<SwaggerScope> Scopes { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string AuthenticationFlow { get; set; }
	}

	public class SwaggerScope
	{
		public string Scope { get; set; }
		public string Description { get; set; }
	}
}
