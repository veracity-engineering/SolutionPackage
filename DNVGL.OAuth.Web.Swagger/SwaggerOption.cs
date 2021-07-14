using System.Collections.Generic;

namespace DNVGL.OAuth.Web.Swagger
{
	public class SwaggerOption
	{
		public bool Enabled { get; set; }
		public string Version { get; set; }
		public string Name { get; set; }
		public string ClientId { get; set; }

		public string AuthorizationEndpoint { get; set; }

		public IEnumerable<ScopeEntry> Scopes { get; set; }
	}

	public class ScopeEntry
	{
		public string Scope { get; set; }
		public string Description { get; set; }
	}
}
