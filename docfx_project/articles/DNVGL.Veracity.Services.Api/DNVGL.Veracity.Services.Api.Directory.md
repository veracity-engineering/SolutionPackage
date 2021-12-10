# DNVGL.Veracity.Services.Api.Directory
Provides a client to resources available under the 'Directory' view point of API v3.

This view point is appropriate for core Veracity applications where resources are not restricted to any context.

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](/articles/PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api.Directory`
```

# Getting started

With the nuget package installed, services may injected for each resource individually by using one or more of the following extension methods from `DNVGL.Veracity.Services.Api.Directory.Extensions` inside the `ConfigureServices` method of your `Startup.cs` file:

| Registration method | Service interface |
|--|--|
| `AddCompanyDirectory(string clientConfigurationName)` | ICompanyDirectory |
| `AddServiceDirectory(string clientConfigurationName)` | IServiceDirectory |
| `AddUserDirectory(string clientConfigurationName)` | IUserDirectory |

> Where `clientConfigurationName` refers to the `Name` inside the configuration section providing the parameters in the form of `OAuthHttpClientFactoryOptions`.

## 1. Configuration
`appsettings.json`
```json
{
	"OAuthHttpClients": [
		...
		{
			"Name": "company-directory",
			"Flow": "ClientCredentials",
			"BaseUri": <BaseUri>
			...
		}
		...
	]
}
```

## 2. Registration
`startup.cs`
```cs
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddCompanyDirectory("company-directory")
	...
}
```

This would then make the service available in the constructor where requested by its interface:

## 3. Request service
`CompanyController.cs`
```cs
private readonly ICompanyDirectory _companyDirectory;
...
public void CompanyController(ICompanyDirectory companyDirectory)
{
	...
	_companyDirectory = companyDirectory ?? throw new ArgumentNullException(nameof(companyDirectory));
	...
}
```

# Resources
- Companies
- Services
- Users

## Companies
| Name | Description |
|--|--|
| `Get(string companyId)` | Retrieves an individual company. |
| `ListUsers(string companyId, int page, int pageSize)` | Retrieves a paginated collection of user references of users affiliated with a company. |

## Services
| Name | Description |
|--|--|
| `Get(string serviceId)` | Retrieves an individual service. |
| `ListUsers(string serviceId, int page, int pageSize)` | Retrieves a paginated collection of user references of users subscribed to a service. |

## Users
| Name | Description |
|--|--|
| `Get(string userId)` | Retrieves an individual user. |
| `ListByUserId(params string[] userIds)` | Retrieves a collection of users where the id is included in the parameters. |
| `ListByEmail(string email)` | Retrieves a collection of user references by a specified email value. |
| `ListCompanies(string userId)` | Retrieves a collection of company references of companies with which a user is affiliated. |
| `ListServices(string userId, int page, int pageSize)` | Retrieves a collection of service references of services to which a user is subscribed. |
| `GetSubscription(string userId, string serviceId)` | Retrieve an individual subscription for a specified user and service. |