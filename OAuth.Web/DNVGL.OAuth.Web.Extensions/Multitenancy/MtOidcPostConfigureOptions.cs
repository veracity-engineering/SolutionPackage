using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace DNV.OAuth.Web.Extensions.Multitenancy;

internal class MtOidcPostConfigureOptions : IPostConfigureOptions<OpenIdConnectOptions>
{
	private readonly OpenIdConnectPostConfigureOptions _impl;

	public MtOidcPostConfigureOptions(IDataProtectionProvider dataProtection)
	{
		_impl = new OpenIdConnectPostConfigureOptions(dataProtection);
	}

	public void PostConfigure(string name, OpenIdConnectOptions options)
	{
		_impl.PostConfigure(name, options);

		options.NonceCookie = new MtCookieBuilder(options.NonceCookie, options);
		options.CorrelationCookie = new MtCookieBuilder(options.CorrelationCookie, options);
	}
}