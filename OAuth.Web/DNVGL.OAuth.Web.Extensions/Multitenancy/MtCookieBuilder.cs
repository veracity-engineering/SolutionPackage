using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Multitenancy;

public class MtCookieBuilder : CookieBuilder
{
	private readonly CookieBuilder _origin;
	private readonly OpenIdConnectOptions _oidcOptions;

	public MtCookieBuilder(CookieBuilder origin, OpenIdConnectOptions oidcOptions)
	{
		_origin = origin;
		_oidcOptions = oidcOptions;
	}

	public override string? Path
	{
		get => _origin.Path;
		set => _origin.Path = value;
	}

	public override string? Domain
	{
		get => _origin.Domain;
		set => _origin.Domain = value;
	}

	public override TimeSpan? Expiration
	{
		get => _origin.Expiration;
		set => _origin.Expiration = value;
	}

	public override bool HttpOnly
	{
		get => _origin.HttpOnly;
		set => _origin.HttpOnly = value;
	}

	public override bool IsEssential
	{
		get => _origin.IsEssential;
		set => _origin.IsEssential = value;
	}

	public override TimeSpan? MaxAge
	{
		get => _origin.MaxAge;
		set => _origin.MaxAge = value;
	}

	public override string? Name
	{
		get => _origin.Name;
		set => _origin.Name = value;
	}

	public override SameSiteMode SameSite
	{
		get => _origin.SameSite;
		set => _origin.SameSite = value;
	}

	public override CookieSecurePolicy SecurePolicy
	{
		get => _origin.SecurePolicy;
		set => _origin.SecurePolicy = value;
	}

	public override CookieOptions Build(HttpContext context, DateTimeOffset expiresFrom)
	{
		var option = _origin.Build(context, expiresFrom);

		PathString path = option.Path;

		if (!string.IsNullOrEmpty(context.Request.PathBase)
		    && path.StartsWithSegments(context.Request.PathBase, out var remainingPath)
		    && remainingPath.StartsWithSegments(_oidcOptions.CallbackPath))
			option.Path = remainingPath;

		return option;
	}
}