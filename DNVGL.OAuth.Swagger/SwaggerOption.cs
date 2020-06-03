namespace DNVGL.OAuth.Swagger
{
	public class SwaggerOption
	{
		public bool Enabled { get; set; }
		public string Version { get; set; }
		public string Name { get; set; }

		public string AuthorizationUrl { get; set; }
		public string[] Scopes { get; set; }
		public string ClientId { get; set; }
	}
}
