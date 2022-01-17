namespace DNVGL.OAuth.Web.Abstractions
{
	public interface IClientAppBuilder
	{
		IClientApp Build(params string[] scopes);
		IClientApp BuildWithOptions(OAuth2Options options);
	}
}
