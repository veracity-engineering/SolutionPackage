# DNVGL.Veracity.Services.Api
DNVGL.Veracity.Services.Api provides a lightweight .NET client for Veracity My Services API v3 built on top of packages from the Solution Package.

This package allows developers to query and manipulate data from My services including user profiles, service profiles, notification messages, company profiles, admin roles and subscriptions.

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](./PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api`
```

# View Points

As a client to API v3, the package is divided in to the following view points:
| Name | Description | Supported authentication |
|--|--|--|
| My | Allows fetching information and making requests for the authenticated user. | User credentials |
| This | Allows manipulation and retrieval of information related to a service. | Client credentials |
| Directory | Allows fetching and updating resources without a focus on a specifc user or service resource. | Client credentials |
