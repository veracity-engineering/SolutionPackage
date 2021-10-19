# User Management and Authorization
DNVGL.Authorization.UserManagement.ApiControllers provides restAPIs to manage user, role and company. It also provides mechanisms to authorize API endpoints.

## Prerequisites
PM> `Install-Package DNVGL.Authorization.UserManagement.ApiControllers`

## Basic Usage
This simple example will show you the minimum steps to setup user management and authorization in a ASP.NET Core project. The example uses SQL Server as database and Veracity authentication (Azure AD B2C).
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
Find and execute `NewTableScript.sql` which is located at the content directory once you imported the package in your project.

### 3. Create master data in database manually. 
#### Setup a company

#### Setup a role

#### Setup a super admin