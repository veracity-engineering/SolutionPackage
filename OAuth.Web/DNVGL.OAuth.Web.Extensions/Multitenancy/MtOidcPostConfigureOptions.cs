using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNV.OAuth.Web.Extensions.Multitenancy;

internal class MtOidcPostConfigureOptions : IPostConfigureOptions<OpenIdConnectOptions>
{
	private readonly OpenIdConnectPostConfigureOptions _decoratedOrigin;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public MtOidcPostConfigureOptions(IDataProtectionProvider dataProtection, IHttpContextAccessor httpContextAccessor)
	{
		_decoratedOrigin = new OpenIdConnectPostConfigureOptions(dataProtection);
        _httpContextAccessor = httpContextAccessor;
    }

	public void PostConfigure(string name, OpenIdConnectOptions options)
	{
		_decoratedOrigin.PostConfigure(name, options);

		options.NonceCookie = new MtCookieBuilder(options.NonceCookie, _httpContextAccessor, options);
		options.CorrelationCookie = new MtCookieBuilder(options.CorrelationCookie, _httpContextAccessor, options);

		var handler = options.Events.OnRedirectToIdentityProvider;

		options.Events.OnRedirectToIdentityProvider = async ctx =>
		{
			if (!string.IsNullOrEmpty(ctx.HttpContext.Request.PathBase)
				&& Uri.TryCreate(ctx.ProtocolMessage.RedirectUri,
				    UriKind.Absolute,
				    out var uri))
			{
				PathString path = uri.PathAndQuery;
				if (!path.StartsWithSegments(ctx.HttpContext.Request.PathBase, out PathString realPath)
				    || string.IsNullOrEmpty(realPath))
					realPath = ctx.HttpContext.Request.PathBase;

				ctx.ProtocolMessage.RedirectUri = new Uri(uri, realPath).AbsoluteUri;
			}

			if (handler != null)
				await handler(ctx);
		};
	}
}