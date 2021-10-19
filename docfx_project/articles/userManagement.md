# User Management and Authorization
DNVGL.Authorization.UserManagement.ApiControllers provides restAPIs to manage user, role and company. It also provides mechanisms to authorize API endpoints.

## Prerequisites
PM> `Install-Package DNVGL.Authorization.UserManagement.ApiControllers`

## Basic Usage
This simple example will show you the minimum steps to setup user management and authorization in a ASP.NET Core project. The example uses SQL Server as database and Veracity authentication (Azure AD B2C).
### 1. register user management module in ASP.NET core project.
PM> `Install-Package Microsoft.EntityFrameworkCore.SqlServer`
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement(
                new UserManagementOptions
                {
                    DbContextOptionsBuilder = options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;")
                });
            //...
        }
    }
```
### 2. Create tables in database
Find and execute `NewTableScript.sql` which is located at the content directory once you imported the package in your project.

### 3. Create a super admin in Table - `Users`. 
The following is sample.
| Id | Email | FirstName | LastName | VeracityId | SuperAdmin | Active | Deleted |
|--|--|--|--|--|--|--|--|
| *1* | *email* | *first name* | *last name* | *veracity id* | 1 | 1 | 0

### 4. Generate Swagger api documentation (Optional)
> **_NOTE:_**  This step is optional. You can generate API docs in your own way. The following code has dependency on Nuget package - `Swashbuckle.AspNetCore`.

```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddSwaggerGen(c =>
            {
                // swagger documentaion group for User Management.
                c.SwaggerDoc("UserManagement", new OpenApiInfo
                {
                    Title = "User Management",
                    Version = "v1"
                });

                // swagger documentaion group for your system.
                c.SwaggerDoc("WebAPI", new OpenApiInfo
                {
                    Title = "Web API",
                    Version = "v1"
                });

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name, api) =>
                {
                    if (name == "UserManagement")
                        return api.GroupName != null && api.GroupName.StartsWith("UserManagement");
                    else
                        return api.GroupName == null;
                });
            });
            //...
        }

        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/UserManagement/swagger.json", "User Management API v1");
                c.SwaggerEndpoint("/swagger/WebAPI/swagger.json", "Web API v1");
            });
            //...
        }
    }
```

### 5. Explore user management APIs
#### Build and Run your project.
#### Open swagger in Browser

![image.png](../images/userManagement/swagger01.png)

### 6. Define permissions
Define permissions by implementing interface - `IPermissionMatrix`. The following code defined two permissions.
```cs
    public class PermissionBook : IPermissionMatrix
    {
        public enum WeatherPermission
        {
            //...

            [PermissionValue(id: "8", key: "ReadWeather", name: "Read Weather", group: "Weather", description: "ReadWeather")]
            ReadWeather,

            [PermissionValue(id: "8", key: "WriteWeather", name: "Write Weather", group: "Weather", description: "WriteWeather")]
            WriteWeather,

            //... other permissions
        }
    }
```
### 7. Authorize API with permissions
Decorates API actions with permission.
```cs
        [HttpGet]
        [PermissionAuthorize(WeatherPermission.ReadWeather)]
        public IEnumerable<WeatherForecast> Get()
        {
            //... api logic
        }
```

## Next Steps
[Use Azure CosmosDB as database](/userManagement/cosmos)

[Use Azure SQL Server as database](/userManagement/sqlserver)

User other databases.
> **_NOTE:_**  The package can use all database engines supported by EF Core 5.0+. Here is a list -  [EF Core 5.0 Database providers](https://docs.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli)