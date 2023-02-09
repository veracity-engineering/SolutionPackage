using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Multitenancy;

public class MtCookieBuilder : CookieBuilder
{
	private readonly CookieBuilder _decoratedOrigin;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly OpenIdConnectOptions _oidcOptions;

	public MtCookieBuilder(CookieBuilder decoratedOrigin, IHttpContextAccessor httpContextAccessor, OpenIdConnectOptions oidcOptions)
	{
		_decoratedOrigin = decoratedOrigin;
		_httpContextAccessor = httpContextAccessor;
		_oidcOptions = oidcOptions;
	}


	public override string? Name
	{
		get
		{
			if(_httpContextAccessor.HttpContext != null && !string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.PathBase))
				return _httpContextAccessor.HttpContext.Request.PathBase.Value.Trim('/') + _decoratedOrigin.Name;
			return _decoratedOrigin.Name;
		}
		set => _decoratedOrigin.Name = value;
	}

	public override string? Path
	{
		get => _decoratedOrigin.Path;
		set => _decoratedOrigin.Path = value;
	}

	public override string? Domain
	{
		get => _decoratedOrigin.Domain;
		set => _decoratedOrigin.Domain = value;
	}

	public override TimeSpan? Expiration
	{
		get => _decoratedOrigin.Expiration;
		set => _decoratedOrigin.Expiration = value;
	}

	public override bool HttpOnly
	{
		get => _decoratedOrigin.HttpOnly;
		set => _decoratedOrigin.HttpOnly = value;
	}

	public override bool IsEssential
	{
		get => _decoratedOrigin.IsEssential;
		set => _decoratedOrigin.IsEssential = value;
	}

	public override TimeSpan? MaxAge
	{
		get => _decoratedOrigin.MaxAge;
		set => _decoratedOrigin.MaxAge = value;
	}

	public override SameSiteMode SameSite
	{
		get => _decoratedOrigin.SameSite;
		set => _decoratedOrigin.SameSite = value;
	}

	public override CookieSecurePolicy SecurePolicy
	{
		get => _decoratedOrigin.SecurePolicy;
		set => _decoratedOrigin.SecurePolicy = value;
	}

	public override CookieOptions Build(HttpContext context, DateTimeOffset expiresFrom)
	{
		var option = _decoratedOrigin.Build(context, expiresFrom);
		
		PathString path = option.Path;

		if (!string.IsNullOrEmpty(context.Request.PathBase))
		{
			if(path.StartsWithSegments(context.Request.PathBase, out var remainingPath)
			   && remainingPath.StartsWithSegments(_oidcOptions.CallbackPath))
			{
				option.Path = remainingPath;
			}
			else
			{
				option.Path = context.Request.PathBase;
			}
		}

		return option;
	}
}