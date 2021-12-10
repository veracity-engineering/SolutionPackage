# DNVGL.Veracity.Services.Api.My
Provides a client to resources available under the 'My' view point of API v3.

This view point is appropriate if you intend to use Veracity as an identity provider for your application.

Resources retrieved from this view point are from the perspective of the authenticated user, **user authentication flow is required to access these resources.**

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](/articles/PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api.My`
```

# Resources
- Companies
- Messages
- Policies
- Profile
- Services

## Companies
| Name | Description |
|--|--|
| `List()` | Retrieves a collection of company references for the authenticated user. |

## Messages
| Name | Description |
|--|--|
| `List(bool includeRead)` | Retrieves a collection of messages addressed to the authenticated user. |
| `Get(string messageId)` | Retrieves an individual message addressed to the authenticated user. |
| `GetUnreadCount()` | Retrieves the numeric value indicating how many messages have not been marked as read by the authenticated user. |

## Policies
| Name | Description |
|--|--|
| `ValidatePolicies(string returnUrl)` | Validates all policies for the authenticated user. |
| `ValidatePolicy(string serviceId, string returnUrl, string skipSubscriptionCheck)` | Validates an individual policy for the authenticated user. | 

## Profile
| Name | Description |
|--|--|
| `Get()` | Retrieves the user profile for the authenticated user. |

## Services
| Name | Description |
|--|--|
| `List()` | Retrieves a collection of service references for services the authenticated user is subscribed to. |
