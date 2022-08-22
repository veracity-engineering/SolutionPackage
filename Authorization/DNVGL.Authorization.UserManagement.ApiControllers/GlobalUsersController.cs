using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.UserManagement.ApiControllers.DTO;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DNVGL.Authorization.Web.PermissionMatrix;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("api/users")]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [ApiExplorerSettings(GroupName = "UserManagement's User APIs")]
    public class GlobalUsersController<TUser> : UserManagementBaseController<TUser> where TUser : User, new()
    {
        private readonly IUser<TUser> _userRepository;
        private readonly PermissionOptions _premissionOptions;
        private readonly IPermissionRepository _permissionRepository;

        public GlobalUsersController(IUser<TUser> userRepository, IUserSynchronization<TUser> userSynchronization, PermissionOptions premissionOptions, IPermissionRepository permissionRepository) : base(userRepository, premissionOptions)
        {
            _userRepository = userRepository;
            _premissionOptions = premissionOptions;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            return await GetAllUsers(_userRepository, _permissionRepository);
        }

        /// <summary>
        /// Get all users, grouping large sets of data into pages.
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser
        /// </remarks>
        /// <param name="page">The page index, starting from 1</param>
        /// <param name="size">the page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{page:int}/{size:int}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<UserViewModel>> GetUsersPaged([FromRoute] int page = 0, [FromRoute] int size = 0)
        {
            return await GetAllUsers(_userRepository, _permissionRepository, page, size);
        }

        /// <summary>
        /// Get a user by user id
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<UserViewModel> GetUser([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return await PopulateUserInfo(user);
        }

        /// <summary>
        /// Get a user by user email
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser
        /// </remarks>
        /// <param name="email">User email</param>
        /// <returns></returns>
        [HttpGet]
        [Route("email/{email}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<UserViewModel> GetUserByEmail([FromRoute] string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            return await PopulateUserInfo(user);
        }



        /// <summary>
        /// Update a user using custom model. Only if custom user model is used.
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageUser
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "email": "",
        ///        "firstName": "",
        ///        "lastName": "",
        ///        "veracityId": "user identity id in Identity Provider",
        ///        "description": "",
        ///        "superAdmin": false,
        ///        "active":true,
        ///        "roleIds":["1","2"],
        ///        "companyIds":["1","2"]
        ///     }
        ///
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        public async Task UpdateUserFromCustomModel([FromRoute] string id, TUser model)
        {
            var currentUser = await GetCurrentUser();
            model.Id = id;
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _userRepository.Update(model);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageUser
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "email": "",
        ///        "firstName": "",
        ///        "lastName": "",
        ///        "veracityId": "user identity id in Identity Provider",
        ///        "description": "",
        ///        "superAdmin": false,
        ///        "active":true,
        ///        "roleIds":["1","2"],
        ///        "companyIds":["1","2"]
        ///     }
        ///
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task UpdateUser([FromRoute] string id, UserEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var user = await _userRepository.Read(id);
            user.Id = id;
            user.Active = model.Active;
            user.Description = model.Description;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.VeracityId = model.VeracityId;
            user.Email = model.Email;
            user.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            user.RoleIds = string.Join(';', model.RoleIds);
            await _userRepository.Update(user);
        }

        /// <summary>
        /// Create a user using custom model. Only if custom user model is used.
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageUser
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "email": "",
        ///        "firstName": "",
        ///        "lastName": "",
        ///        "veracityId": "user identity id in Identity Provider",
        ///        "description": "",
        ///        "superAdmin": false,
        ///        "active":true,
        ///        "roleIds":["1","2"],
        ///        "companyIds":["1","2"]
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("custommodel")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        public async Task<string> CreateUserFromCustommodel([FromBody] TUser model)
        {
            var currentUser = await GetCurrentUser();
            model.CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            model = await _userRepository.Create(model);
            return model.Id;
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageUser
        /// 
        /// Sample request:
        ///
        ///     {
        ///        "email": "",
        ///        "firstName": "",
        ///        "lastName": "",
        ///        "veracityId": "user identity id in Identity Provider",
        ///        "description": "",
        ///        "superAdmin": false,
        ///        "active":true,
        ///        "roleIds":["1","2"],
        ///        "companyIds":["1","2"]
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task<string> CreateUser([FromBody] UserEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var user = new TUser
            {
                Description = model.Description,
                FirstName = model.FirstName,
                LastName = model.LastName,
                VeracityId = model.VeracityId,
                Active = model.Active,
                RoleIds = string.Join(';', model.RoleIds),
                Email = model.Email,
                CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}",
            };

            user = await _userRepository.Create(user);
            return user.Id;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <remarks>
        /// Required Permissions: ManageUser
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task DeleteUser([FromRoute] string id)
        {
            await _userRepository.Delete(id);
        }

        /// <summary>
        /// Get current user info.
        /// </summary>
        /// <remarks>
        /// No Required Permissions
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/users/currentUser")]
        public async Task<UserViewModel> GetUserByIdentityId()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext.User);
            var user = await _userRepository.ReadByIdentityId(varacityId);
            return await PopulateUserInfo(user);
        }

        /// <summary>
        /// Get a user's all permissions
        /// </summary>
        /// <remarks>
        /// No Required Permissions
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/users/{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<string>> GetUserCorssCompanyPermissions([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            if (user.SuperAdmin)
            {
                return (await _permissionRepository.GetAll()).Select(t => t.Key);
            }
            return user.RoleList.Where(t=>t.Active).SelectMany(t => t.PermissionKeys);
        }



        private async Task<UserViewModel> PopulateUserInfo(TUser user)
        {
            if(user == null) return null;
            var allPermissions = await _permissionRepository.GetAll();
            var result = user.ToViewDto<UserViewModel>();
            result = PopulateUserRoleInfo(user, result, allPermissions);
            return result;
        }

    }
}
