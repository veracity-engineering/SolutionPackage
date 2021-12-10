# DNVGL.Veracity.Services.Api.Directory
Provides a client to resources available under the 'Directory' view point of API v3.

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](./PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api.Directory`
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