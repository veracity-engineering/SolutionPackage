# HTTP Response Security Header
DNVGL.Web.Security provides extension methods to setup http response headers for ASP.NET Core application.

## Prerequisites
PM> `Install-Package DNVGL.Web.Security`

## 1. Basic Example
```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders();
            //...
        }
    }
```

* The package set up below default http response headers.

| Key | Value 
|---|---
|X-Xss-Protection|  1
|X-Frame-Options|SAMEORIGIN
|X-Content-Type-Options| no-referrer
|X-Permitted-Cross-Domain-Policies|none
|Expect-CT|enforce, max-age=7776000
|Strict-Transport-Security|max-age=15552000; includeSubDomains  
>If you have setup your own response headers before using the pacakge to setup default headers. You own reponse headers will be kept.  


* The package set up below default csp rule in http response headers.  

| Key | Value 
|---|---
|default-src|'self'
|object-src|'self'
|connect-src|'self' https://dc.services.visualstudio.com
|script-src|'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn
|font-src|'self' data: https://onedesign.azureedge.net
|media-src|'self'
|worker-src|'self' blob:
|img-src|'self' data: https://onedesign.azureedge.net
|frame-src|'self' https://www.google.com https://www.recaptcha.net/
|style-src|'self' https://onedesign.azureedge.net

>If you have setup your own CSP before using the pacakge to setup default headers. You own CSP will be kept.  

## 2. Customize Response Header
The pacakge supports to overwrite the above default setting. This is a code sample to overwrite X-Frame-Options:

```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.Add("X-Frame-Options", "DENNY"));
            //...
        }
    }
```

## 3. Customize CSP in Response Header
The pacakge supports to overwrite the above default setting. This is a code sample to overwrite styleSrc:
 ```cs
     public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.AddContentSecurityPolicy(styleSrc: "'self' 'nonce-123456789909876543ghjklkjvcvbnm'"););
            //...
        }
    }
 ```

 ## 4. Skip CSP in Response Header for specific requests.
By default, The package doesn't add csp into respsone for all http requests which url contain '/swagger/'.
It supports to overwrite the default skip logic. This is a code sample to skip all request which url contains '/nocsprequired/'.
```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.SkipContentSecurityPolicyForRequests((req) => req.Path.ToString().ToLowerInvariant().Contains("/nocsprequired/")));
            //...
        }
    }
```