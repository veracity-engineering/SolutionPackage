# Overview
DNVGL.OAuth.Api.HttpClient package has two credentials. One is user credentials which use current user's credentials to access api. Other is client credentials which is a service to service model to access api. 

# Package Install

To install DNVGL.OAuth.Api.HttpClient package, you may need to add the below package feed to your nuget sources.

```
https://dnvgl-one.pkgs.visualstudio.com/_packaging/DNVGL.SolutionPackage/nuget/v3/index.json
```

Package Manager Console
```
PM> `Install-Package DNVGL.OAuth.Api.HttpClient`
```
Or Package Manager for solution/project
![](../images/dnvgl.oauth.api.httpclient/add-package.png)

# Basic example
```js
  {
    "ApiHttpClientOptions":[
      {
        "Name": "userCredentialsClient",
        "Flow":"user-credentials",
        "BaseUri": "https://localhost/api/user",
        "SubscriptionKey": "eqrqie3431qre234"
      },
      {
        "Name": "clientCredentialsClient",
        "Flow":"client-credentials",
        "BaseUri": "https://localhost/api/client",
        "SubscriptionKey": "eqrqisfs34s1qre734"
      }
    ]

  }

```

```cs
public void ConfigureService(IServiceCollection services)
{
  ...
  services.AddOAuthHttpClientFactory(Congiuration.GetSection("ApiHttpClientOptions").Get<IEnumerable<OAuthHttpClientFactoryOptions>>());
  ...

}
```

```cs

public class TestController
{
  private readonly IOAuthHttpClientFactory _oauthHttpClientFactory;

  public TestController(IOAuthHttpClientFactory httpClientFactory)
  {
    _oauthHttpClientFactory = httpClientFactory;
  }

  public User GetUser(string id)
  {
    var client = _oauthHttpClientFactory.Create('userCredentialsClient');
    ...
  }

  public Company GetCompany(string id)
  {
    var client = _oauthHttpClientFactory.Create('clientCredentialsClient');
    ...
  }
}

```





