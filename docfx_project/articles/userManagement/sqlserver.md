# Use SQL Server as database

## Prerequisites
PM> `Install-Package Microsoft.EntityFrameworkCore.SqlServer`

## Register user management module in ASP.NET core project
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
                    DbContextOptionsBuilder = options => options.UseCosmos("***Connection string***", "UserManagement"),
                    ModelBuilder = (modelBuilder) => modelBuilder.HasDefaultContainer("User"),
                });
            //...
        }
    }
```

### 2. Create tables in database
Find and execute `NewTableScript.sql` which is located at the Content directory once you imported the package in your project.

### 3. Create a super admin in Table - `Users`. 
The following is sample.
| Id | Email | FirstName | LastName | VeracityId | SuperAdmin | Active | Deleted |
|--|--|--|--|--|--|--|--|
| *1* | *email* | *first name* | *last name* | *veracity id* | 1 | 1 | 0