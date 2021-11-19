namespace DNVGL.OAuth.Web.Abstractions
{
	public interface IClientAppBuilder
	{
		IClientApp Build(params string[] scope);
		IClientApp BuildWithOptions(OAuth2Options options);
	}
}
