using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNV.OAuth.Web.Extensions.Multitenancy;

internal class MtCookieConfigureNamedOptions
	: IConfigureNamedOptions<CookieAuthenticationOptions>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly Action<CookieAuthenticationOptions, HttpContext> _configureAction;

	public MtCookieConfigureNamedOptions(
		IHttpContextAccessor httpContextAccessor,
		Action<CookieAuthenticationOptions, HttpContext> configureAction)
	{
		_httpContextAccessor = httpContextAccessor;
		_configureAction = configureAction;
	}

	public void Configure(string name, CookieAuthenticationOptions options)
	{
		if (!string.Equals(name, CookieAuthenticationDefaults.AuthenticationScheme, StringComparison.Ordinal))
		{
			return;
		}

		if (_httpContextAccessor.HttpContext == null)
		{
			throw new InvalidOperationException("HttpContext is not available.");
		}

		_configureAction(options, _httpContextAccessor.HttpContext);
	}

	public void Configure(CookieAuthenticationOptions options)
		=> Configure(Options.DefaultName, options);
}