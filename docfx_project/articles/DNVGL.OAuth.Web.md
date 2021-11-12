[TOC]

# Overview

DNVGL.OAuth.Web is a .NETCore library for developers to simplify the work of setting up OpenId Connection authentication (OIDC) such as Veracity or Azure AD B2C for ASP.NET Core web project.

---

# Package Install

To install the DNVGL.OAuth.Web package, you may need to add the package feed below to your nuget sources.

```
https://dnvgl-one.pkgs.visualstudio.com/_packaging/DNVGL.SolutionPackage/nuget/v3/index.json
```

![](../images/DNVGL.OAuth.Web/add-feed.png)

Search nuget package of DNVGL.OAuth.Web and add it to your project.
![](../images/DNVGL.OAuth.Web/download-package.png)

---

# Authentication for websites

To simplify your authentication implementation of Veracity for your ASP.NET Core website, you need to add 3 blocks of codes to `Startup.cs`.

1. Add namespace reference. 

```csharp
using DNVGL.OAuth.Web;
```

2. Add `AddOidc` extension method into `ConfigureServices`.
```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	// implicit flow, the most simple way
	services.AddOidc(o =>
	{
		o.Authority = "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0";
		o.ClientId = "<ClientId>";
	});

	// authorization code flow, the better way
	services.AddOidc(o =>
	{
		o.Authority = "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0";
		o.ClientId = "<ClientId>";
		o.ClientSecret = "<ClientSecret>";
		o.ResponseType = "code";
	});
	...
}
```

3. Add `UseAuthentication` and `UseAuthorization` extension methods into `Configure`.
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	...
	app.UseAuthentication().UseAuthorization();
	...
}
```

4. Then you could launch your project and access an action in a controller that decorated with `[Authorize]`, a challenge request will be sent to IDP(Microsoft or Veracity) to start the authentication process, and the `HttpContext` will be filled with authentication result. 

```csharp
[Authorize]
public class HomeController : Controller
{
}
```

![](../images/DNVGL.OAuth.Web/challenge.png)

Beware of the usage of `[Authorize]` attribute, and do have an authentication failure plan, otherwise you might run into a re-authenticate loop. 🤣

5. A sample project is ready for you to try out: [SimpleOAuthSample](https://dnvgl-one.visualstudio.com/Innersource/_git/DNVGL.SolutionPackage.Demo?path=/SimpleOAuthSample).

---

# Authentication for Web APIs

Unlike the website, web API expect every incoming request with an access token tagged alone, and web API will not help you to get a token. You get either a successful access or a 401.

1. Add namespace reference. 

```csharp
using DNVGL.OAuth.Web;
```

2. Add `AddJwt` extension method into `ConfigureServices`.
```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	// call AddAuthentication before AddJwt.
	services.AddAuthentication("Bearer")
		.AddJwt("Bearer", o =>
		{
			o.Authority = "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/	b2c_1a_signinwithadfsidp/v2.0";
			o.ClientId = "<ClientId>";
		});
	...
}
```

3. Add `UseAuthentication` and `UseAuthorization` extension methods into `Configure`.
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	...
	app.UseAuthorization();
	...
}
```

4. Then decorate you API controller or endpoint with `[Authorize]` attribute and specify the authentication scheme.

```csharp
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class ApiController : ControllerBase
	{
	}
```

5. A sample project is ready for you to try out: [SimpleOAuthSample](https://dnvgl-one.visualstudio.com/Innersource/_git/DNVGL.SolutionPackage.Demo?path=/SimpleOAuthSample).

---

# Access Token Cache Usage

If you web project act as an API gateway, you will want to cache users' access tokens to prevent unnecessary token requests. The library uses `MSAL (Microsoft Authentication Library)` to manipulate tokens.

1. Authorization code flow needs to be set to acquire access token, and refresh token is required for MSAL to re-acquire token from IDP if the token exceed its expiration.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	var oidcOptions = new OidcOptions
	{
		ClientId = "<ClientId>",
		ClientSecret = "<ClientSecret>",
		Scopes = new[] { "<Scope>", "offline_access" },	// offline_access is required to retrieve refresh_token.
		ResponseType = "code""
	};
	...
}
```

2. To cache the tokens, an implementaion of `IDistributedCache` such as `MemoryDistributedCache` needs to be added.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddDistributedMemoryCache();
	...
}
```

You can also add `RedisCache` instead.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddDistributedRedisCache(o =>
	{
		o.InstanceName = "<InstanceName>";
		o.Configuration = "<Configuration>";
	});
	...
}
```

3. Calling `AddDistributedTokenCache` will have `IDistributedCache` attached to MSAL client app behind the scene, and the token acquiring process will be replaced by MSAL client app.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddDistributedTokenCache(oidcOptions);
	...
}
```

4. Don't forget to add `AddOidc` after what you did previously.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddOidc(oidcOptions);
	...
}
```

5. A sample project is ready for you to try out: [TokenCacheDemo](//TokenCacheDemo).

---
