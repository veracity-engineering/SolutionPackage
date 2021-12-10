# DNVGL.Veracity.Services.Api.This
Provides a client to resources available under the 'This' view point of API v3.

Resources retrieved from this view point are from the perspective of a service, **client credential authentication flow is required to access these resources.**

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](./PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api.This`
```

# Resources
- Administrators
- Services
- Subscribers
- Users

## Administrators
| Name | Description |
|--|--|
| `Get(string userId)` | Retrieves an individual administrator for the authenticated service. |
| `List(int page, int pageSize)` | Retrieves a collection of administrator references for the authenticated service. |

## Services
| Name | Description |
|--|--|
| `AddSubscription(string serviceId, string userId, SubscriptionOptions options)` | Add a subscription to the authenticated service or nested services. |
| `GetAdministrator(string serviceId, string userId)` | Retrieve an individual administrator reference to a administrator of the authenticated service or nested services. |
| `GetSubscriber(serviceId, tring userId)` | Retrieve an individual user reference to a user which has a subscription to a specified service. |
| `List(int page, int pageSize)` | Retrieve a collection of services the authenticated service has access to. |
| `ListAdministrators(string serviceId, int page int pageSize)` | Retrieve a collection of administrator references of administrators for a specified service. |
| `ListSubscribers(string serviceId, int page, int pageSize)` | Retrieve a collection of user references of users subscribed to a specified service. |
| `NotifySubscribers(string serviceId, NotificationOptions options)` | Send a notification to users subscribed to the authenticated service or nested service. |
| `RemoveSubscription(string servieId, string userId)` | Remove a user subscription for a user and the authenticated service or a nested service. |

## Subscribers
| Name | Description |
|--|--|
| `Add(string userId, SubscriptionOptions options)` | Add a subscription to the authenticated service for a specified user. |
| `Get(string userId)` | Retrieve a user reference for a user subscribed to the authenticated service. |
| `List(int page, int pageSize)` | Retrieve a collection of user references to users subscribed to the authenticated service. |
| `Remove(string userId)` | Remove a user subscription to the authenticated service by specified user. |

## Users
| Name | Description |
|--|--|
| `Create(CreateUserOptions options)` | Create a new user. |
| `Create(params CreateUserOptions[] options)` | Create a collection of new users. |
| `Resolve(string email)` | Retrieves a collection of user references for users with a specified email value. |