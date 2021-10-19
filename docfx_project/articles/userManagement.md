# User Management and Authorization
DNVGL.Authorization.UserManagement.ApiControllers provides restAPIs to manage user, role and company. It also provides mechanisms to authorize API endpoints.

## Prerequisites
PM> `Install-Package DNVGL.Authorization.UserManagement.ApiControllers`

## Setup a basic example
### 1. register user management module in ASP.NET core.
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
Find and execute NewTableScript.sql which is located at the content directory once you imported the package in your project.

### 3. Create master data in database manually. 