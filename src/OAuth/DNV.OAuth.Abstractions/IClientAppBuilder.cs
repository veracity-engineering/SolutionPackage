namespace DNV.OAuth.Abstractions
{
	public interface IClientAppBuilder
	{
		IClientApp Build(OAuth2Options options);
	}
}
