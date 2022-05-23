using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace DNV.OAuth.Web.Extensions.Multitenancy;

internal class MtCookieOptionsMonitor : IOptionsMonitor<CookieAuthenticationOptions>
{
	private readonly IOptionsFactory<CookieAuthenticationOptions> _optionsFactory;

	public MtCookieOptionsMonitor(IOptionsFactory<CookieAuthenticationOptions> optionsFactory)
	{
		_optionsFactory = optionsFactory;
	}

	public CookieAuthenticationOptions CurrentValue => Get(Options.DefaultName);

	public CookieAuthenticationOptions Get(string name)
	{
		return _optionsFactory.Create(name);
	}

	public IDisposable? OnChange(Action<CookieAuthenticationOptions, string> listener) => null;
}