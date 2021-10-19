# Overview
In this section, you learn the different modes of user management, permission setup, package limitations, and decide the adoption of the package. 

## 1. Three user management modes
> one user is allowed to be assigned to mutiple companies. `Company_CompanyRole_User` is the default mode.

| Mode | Company | Role | User
|--|--|--|--|
| `Company_CompanyRole_User`| &check; | &check; Role is defined at company level. | &check; |
| `Company_GlobalRole_User`| &check; | &check; Role is defined at global level. | &check; |
| `Role_User`| &#9746; | &check; | &check; |

### Change default user management mode
> API endpoint and schema has slight difference on different modes.

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
                    //...
                    Mode = UserManagementMode.Role_User
                });
            //...
        }
    }
```

## 2. Permission setup
### Build-in Permissions
The following are serveral built-in permissions used to authorize user management apis.
| Permission | Description |
|--|--|
| `Premissions.ManageUser` | permission to make change on user. |
| `Premissions.ViewUser` | permission to read user. |
| `Premissions.ManageRole` | permission to make change on role. |
| `Premissions.ViewRole` | permission to read role. |
| `Premissions.ManageCompany` | permission to make change on company. |
| `Premissions.ViewCompany` | permission to read role. |

### Create your own premissions
Define permissions by implementing interface - `IPermissionMatrix`. The following code defined two permissions.
> You are also allowed to manage permissions outside of the source code. [Here is the instruction](/permissionStore).
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

### Authorize API with permissions
Decorates API actions with permission.
>  You are also allowed to use Role-based authorization like `[Authorize(Roles = "****")]`. [Here is the instruction](/authorize).
```cs
        [HttpGet]
        [PermissionAuthorize(WeatherPermission.ReadWeather)]
        public IEnumerable<WeatherForecast> Get()
        {
            //... api logic
        }
```

## 3. Company/ Role/ User deletion
So far, the soft deletion is not yet implemented, however we have reserved the `Deleted` in the data model for this purpose. In another word, record will be **HARD DELETED** once you call delete api. 

To implement **SOFT DELETE**, you can utilize `Active` field in the data model with update APIs as of now.

## 4. Data Access Implementation
The package has dependency on EF Core 5.0+. If you don't want to 