namespace DNV.OAuth.Abstractions
{
	public interface IClientAppFactory
	{
		IClientApp CreateForUser(string scope);
		IClientApp CreateForClient(string scope);
	}
}
