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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DNVGL.Authorization.Web.PermissionMatrix;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [TypeFilter(typeof(ErrorCodeExceptionFilter))]
    [Route("api/company/{companyId}/users")]
    [CompanyIdentityFieldNameFilter(companyIdInRoute: "companyId")]
    [ApiExplorerSettings(GroupName = "UserManagement's User APIs")]
    public class UsersController<TRole, TUser> : UserManagementBaseController<TUser> where TRole : Role, new() where TUser : User, new()
    {
        private readonly IRole<TRole> _roleRepository;
        private readonly IUser<TUser> _userRepository;
        private readonly PermissionOptions _premissionOptions;
        private readonly IPermissionRepository _permissionRepository;
        private readonly UserManagementSettings _userManagementSettings;

        public UsersController(IUser<TUser> userRepository, IRole<TRole> roleRepository, IUserSynchronization<TUser> userSynchronization
            , PermissionOptions premissionOptions, IPermissionRepository permissionRepository, UserManagementSettings userManagementSettings) : base(userRepository, premissionOptions)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _premissionOptions = premissionOptions;
            _permissionRepository = permissionRepository;
            _userManagementSettings = userManagementSettings;
        }

        /// <summary>
        /// Get all user of a company
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewUser 
        /// 
        /// Required Permission for user not in this company: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<IEnumerable<UserViewModel>> GetUsers([FromRoute] string companyId)
        {
            return await GetUsersOfCompany(companyId);
        }

        /// <summary>
        /// Get all user of a company, grouping large sets of data into pages.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewUser 
        /// 
        /// Required Permission for user not in this company: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <param name="page">The page index, starting from 1</param>
        /// <param name="size">the page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{page:int}/{size:int}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<IEnumerable<UserViewModel>> GetUsersPaged([FromRoute] string companyId, [FromRoute] int page = 0, [FromRoute] int size = 0)
        {
            return await GetUsersOfCompany(companyId,page,size);
        }

        /// <summary>
        /// Get user info by user id
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewUser 
        /// 
        /// Required Permission for user not in this company: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<UserViewModel> GetUser([FromRoute] string companyId, [FromRoute] string id)
        {
            var user = await GetUserById(id);

            if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
                user = PruneUserInfo(user, companyId);
            else
                user = PruneUserCompanyInfo(user, companyId);

            if (user != null && user.Companies.Any(t => t.Id == companyId))
                return user;

            return null;
        }

        /// <summary>
        /// Get user info by user email
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewUser 
        /// 
        /// Required Permission for user not in this company: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <param name="email">User email</param>
        /// <returns></returns>
        [HttpGet]
        [Route("email/{email}")]
        [PermissionAuthorize(Premissions.ViewUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<UserViewModel> GetUserByEmail([FromRoute] string companyId, [FromRoute] string email)
        {
            var user = await GetUserByEmail(email);

            if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
                user = PruneUserInfo(user, companyId);
            else
                user = PruneUserCompanyInfo(user, companyId);

            if (user != null && user.Companies.Any(t => t.Id == companyId))
                return user;

            return null;
        }


        /// <summary>
        /// Update a user using custom model. Only if custom user model is used.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageUser 
        /// 
        /// Required Permission for user not in this company: ManageUser,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="id">User Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        public async Task UpdateUserFromCustomModel([FromRoute] string companyId, [FromRoute] string id, TUser model)
        {
            var currentUser = await GetCurrentUser();
            var user = await _userRepository.Read(id);

            model.Id = user.Id;
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";

            var roleIds = await PruneRoles(companyId, model.RoleIds.SplitToList(';'));

            if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
            {
                roleIds = roleIds.Concat(user.RoleList.Where(t => t.CompanyId != companyId).Select(t => t.Id)).ToList();
            }

            model.RoleIds = string.Join(';', roleIds);

            if (!model.CompanyIdList.Contains(companyId))
            {
                model.CompanyIds = model.CompanyIds + ";" + companyId;
            }

            await _userRepository.Update(model);

        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageUser 
        /// 
        /// Required Permission for user not in this company: ManageUser,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="id">User Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task UpdateUser([FromRoute] string companyId, [FromRoute] string id, UserEditModel model)
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

            var roleIds = await PruneRoles(companyId, model.RoleIds);

            if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
            {
                roleIds = roleIds.Concat(user.RoleList.Where(t => t.CompanyId != companyId).Select(t => t.Id)).ToList();
            }

            user.RoleIds = string.Join(';', roleIds);

            if (!user.CompanyIdList.Contains(companyId))
            {
                user.CompanyIds = user.CompanyIds + ";" + companyId;
            }

            await _userRepository.Update(user);
        }

        /// <summary>
        /// Create a user using custom model. Only if custom user model is used.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageUser 
        /// 
        /// Required Permission for user not in this company: ManageUser,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("custommodel")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<string> CreateUserFromCustommodel([FromRoute] string companyId, [FromBody] TUser model)
        {
            var currentUser = await GetCurrentUser();
            var user = await _userRepository.ReadByIdentityId(model.VeracityId);
            var roleIds = await PruneRoles(companyId, model.RoleIds.SplitToList(';'));
            if (user == null)
            {
                model.CompanyIds = companyId;
                model.RoleIds = string.Join(';', roleIds);
                model.CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
                model = await _userRepository.Create(model);
            }
            else
            {
                await UpdateUserFromCustomModel(companyId, user.Id, model);
            }
            return model.Id;
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageUser 
        /// 
        /// Required Permission for user not in this company: ManageUser,ViewCompany
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
        /// <param name="companyId">Company Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<string> CreateUser([FromRoute] string companyId, [FromBody] UserEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var user = await _userRepository.ReadByIdentityId(model.VeracityId);
            var roleIds = await PruneRoles(companyId, model.RoleIds);
            if (user == null)
            {
                user = new TUser
                {
                    Description = model.Description,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    VeracityId = model.VeracityId,
                    Active = model.Active,
                    CompanyIds = companyId,
                    RoleIds = string.Join(';', roleIds),
                    Email = model.Email,
                    CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}",
                };

                user = await _userRepository.Create(user);

            }
            else
            {
                await UpdateUser(companyId,user.Id,model);
            }
            return user.Id;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ManageUser 
        /// 
        /// Required Permission for user not in this company: ManageUser,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [PermissionAuthorize(Premissions.ManageUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task DeleteUser([FromRoute] string companyId, [FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            var currentUser = await GetCurrentUser();

            if (user.CompanyIds == companyId)
            {
                await _userRepository.Delete(id);
            }
            else if (user.CompanyIdList.Contains(companyId))
            {
                var companyIds = user.CompanyIdList.Where(t => t != companyId).ToList();
                var roleIds = user.RoleList.Where(t => t.CompanyId != companyId).Select(t => t.Id).ToList();
                user.CompanyIds = string.Join(';', companyIds);

                if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
                    user.RoleIds = string.Join(';', roleIds);

                user.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
                await _userRepository.Update(user);
            }
        }

        /// <summary>
        /// Get Current user info with user's permission in this company.
        /// </summary>
        /// <remarks>
        /// No permission is required
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/company/{companyId}/users/currentUser")]
        public async Task<UserViewModel> GetCompanyUserByIdentityId([FromRoute] string companyId)
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext.User);
            var user = await GetUserByIdentityId(varacityId);
            if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
                user = PruneUserInfo(user, companyId);
            else
                user = PruneUserCompanyInfo(user, companyId);
            return user;
        }

        /// <summary>
        /// Ger a user's permission in this company.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewUser 
        /// 
        /// Required Permission for user not in this company: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="companyId">Company Id</param>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/company/{companyId}/users/{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        public async Task<IEnumerable<string>> GetUserPermissions([FromRoute] string companyId, [FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return user.RoleList.Where(t => t.CompanyId == companyId).SelectMany(t => t.PermissionKeys);
        }

        /// <summary>
        /// Get Current user info with user's all permission 
        /// </summary>
        /// <remarks>
        /// No permission is required
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/users/currentUser")]
        public async Task<UserViewModel> GetUserByIdentityId()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext.User);
            return await GetUserByIdentityId(varacityId);
        }

        /// <summary>
        /// Get a user's all permissions.
        /// </summary>
        /// <remarks>
        /// Required Permission for user in the this company: ViewUser 
        /// 
        /// Required Permission for user not in this company: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/users/{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        public async Task<IEnumerable<string>> GetUserCorssCompanyPermissions([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return user.RoleList.SelectMany(t => t.PermissionKeys);
        }

        /// <summary>
        /// Get all users. 
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser,ViewCompany
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/admin/users")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<IEnumerable<UserViewModel>> GetCrossCompanyUsers()
        {
            return await GetAllUsers(_userRepository, _permissionRepository);
        }

        /// <summary>
        /// Get all users, grouping large sets of data into pages.
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="page">The page index, starting from 1</param>
        /// <param name="size">the page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/admin/users/{page:int}/{size:int}")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<IEnumerable<UserViewModel>> GetCrossCompanyUsersGetUsersPaged([FromRoute] int page = 0, [FromRoute] int size = 0)
        {
            return await GetAllUsers(_userRepository, _permissionRepository,page,size);
        }


        [HttpGet]
        [Route("~/api/admin/{companyid}/users")]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<IEnumerable<UserViewModel>> GetCompanyUsers([FromRoute] string companyid)
        {
            return await GetUsersOfCompany(companyid);
        }

        /// <summary>
        /// Get a user's info. Only using it if user's company is unknown.
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/admin/users/{id}")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<UserViewModel> GetCrossCompanyUser([FromRoute] string id)
        {
            return await GetUserById(id);
        }

        /// <summary>
        /// Get a user's info. Only using it if user's company is unknown.
        /// </summary>
        /// <remarks>
        /// Required Permission: ViewUser,ViewCompany
        /// </remarks>
        /// <param name="email">User Email</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/admin/users/email/{email}")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<UserViewModel> GetCrossCompanyUserByEmail([FromRoute] string email)
        {
            return await GetUserByEmail(email);
        }



        /// <summary>
        /// Create a user using custom model. Only if custom user model is used. Only using it if user's company is unknown.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageUser, ViewCompany 
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
        [Route("~/api/admin/users/custommodel")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [PermissionAuthorize(Premissions.ManageUser, Premissions.ViewCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        public async Task<ActionResult> CreateCrossCompanyUserFromCustomModel([FromBody] TUser model)
        {
            var user = await _userRepository.ReadByIdentityId(model.VeracityId);
            if (user != null)
            {
                return BadRequest("User alreay exists.");
            }

            var currentUser = await GetCurrentUser();
            var roleIds = await PruneRoles(model.CompanyIds, model.RoleIds.SplitToList(';'));
            model.RoleIds = string.Join(';', roleIds);
            model.CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            model = await _userRepository.Create(model);
            return Ok(model.Id);
        }

        /// <summary>
        /// Create a user. Only using it if user's company is unknown.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageUser, ViewCompany 
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
        [Route("~/api/admin/users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [PermissionAuthorize(Premissions.ManageUser, Premissions.ViewCompany)]
        public async Task<ActionResult> CreateCrossCompanyUser([FromBody] UserEditModel model)
        {
            var user = await _userRepository.ReadByIdentityId(model.VeracityId);
            if (user != null)
            {
                return BadRequest("User alreay exists.");
            }

            var currentUser = await GetCurrentUser();
            var roleIds = await PruneRoles(model.CompanyIds, model.RoleIds);
            user = new TUser
            {
                Description = model.Description,
                FirstName = model.FirstName,
                LastName = model.LastName,
                VeracityId = model.VeracityId,
                Active = model.Active,
                SuperAdmin=model.SuperAdmin,
                CompanyIds = string.Join(';', model.CompanyIds),
                RoleIds = string.Join(';', roleIds),
                Email = model.Email,
                CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}",
            };
            user = await _userRepository.Create(user);
            return Ok(user.Id);
        }

        /// <summary>
        /// Update a user using custom model. Only if custom user model is used. Only using it if user's company is unknown.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageUser, ViewCompany 
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
        [Route("~/api/admin/users/custommodel/{id}")]
        [PermissionAuthorize(Premissions.ManageUser, Premissions.ViewCompany)]
        [ApiExplorerSettings(GroupName = "UserManagement's User APIs - Custom Model")]
        public async Task UpdateCrossCompanyUserFromCustomModel([FromRoute] string id, TUser model)
        {
            var currentUser = await GetCurrentUser();
            var roleIds = await PruneRoles(model.CompanyIds, model.RoleIds.SplitToList(';'));
            var user = await _userRepository.Read(id);
            model.Id = user.Id;
            model.RoleIds = string.Join(';', roleIds);
            model.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _userRepository.Update(model);
        }


        /// <summary>
        /// Update a user.Only using it if user's company is unknown.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageUser, ViewCompany 
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
        [Route("~/api/admin/users/{id}")]
        [PermissionAuthorize(Premissions.ManageUser, Premissions.ViewCompany)]
        public async Task UpdateCrossCompanyUser([FromRoute] string id, UserEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var roleIds = await PruneRoles(model.CompanyIds, model.RoleIds);
            var user = await _userRepository.Read(id);
            user.Id = id;
            user.Active = model.Active;
            user.SuperAdmin = model.SuperAdmin;
            user.Description = model.Description;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.VeracityId = model.VeracityId;
            user.CompanyIds = string.Join(';', model.CompanyIds);
            user.RoleIds = string.Join(';', roleIds);
            user.Email = model.Email;
            user.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
            await _userRepository.Update(user);
        }


        /// <summary>
        /// Delete a user.Only using it if user's company is unknown.
        /// </summary>
        /// <remarks>
        /// Required Permission: ManageUser, ViewCompany 
        /// </remarks>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("~/api/admin/users/{id}")]
        [PermissionAuthorize(Premissions.ManageUser, Premissions.ViewCompany)]
        public async Task DeleteCrossCompanyUser([FromRoute] string id)
        {
            await _userRepository.Delete(id);
        }

        private async Task<IList<string>> PruneRoles(string companyId, IList<string> sourceRoleIds)
        {
            if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
            {
                var roles = await _roleRepository.GetRolesOfCompany(companyId);
                return sourceRoleIds.Where(t => roles.Any(f => f.Id == t)).ToList();
            }
            else
                return sourceRoleIds;
        }

        private async Task<IList<string>> PruneRoles(IList<string> companyIds, IList<string> sourceRoleIds)
        {
            if (_userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
            {
                var roles = new List<TRole>();
                foreach (var companyId in companyIds)
                {
                    roles.AddRange(await _roleRepository.GetRolesOfCompany(companyId));
                }
                return sourceRoleIds.Where(t => roles.Any(f => f.Id == t)).ToList();
            }
            else
                return sourceRoleIds;
        }

        private async Task<IEnumerable<UserViewModel>> GetUsersOfCompany(string companyId, int page = 0, int size = 0)
        {
            var users = await _userRepository.GetUsersOfCompany(companyId,page,size);
            var allPermissions = await _permissionRepository.GetAll();

            var result = users.Select(t =>
            {
                var dto = t.ToViewDto<UserViewModel>();

                if (t.RoleList != null)
                {
                    dto.Roles = t.RoleList.Where(r => r.CompanyId == companyId || _userManagementSettings.Mode == UserManagementMode.Company_GlobalRole_User).Select(r =>
                          {
                              var RoleViewDto = r.ToViewDto<RoleViewDto>();

                              if (r.PermissionKeys != null)
                              {
                                  RoleViewDto.Permissions = allPermissions.Where(p => r.PermissionKeys.Contains(p.Key));
                              }

                              return RoleViewDto;
                          });
                }

                return dto;
            });


            return result;
        }

        private async Task<UserViewModel> GetUserByIdentityId(string varacityId)
        {
            var user = await _userRepository.ReadByIdentityId(varacityId);
            return await PopulateUserInfo(user);
        }

        private async Task<UserViewModel> GetUserById(string userId)
        {
            var user = await _userRepository.Read(userId);
            return await PopulateUserInfo(user);
        }

        private async Task<UserViewModel> GetUserByEmail(string email)
        {
            var userEntity = await _userRepository.GetUserByEmail(email);
            return await PopulateUserInfo(userEntity);
        }



        private async Task<UserViewModel> PopulateUserInfo(TUser user)
        {
            if (user == null) return default;

            var allPermissions = await _permissionRepository.GetAll();
            var result = user.ToViewDto<UserViewModel>();
            result = PopulateUserRoleInfo(user, result, allPermissions);

            if (user.CompanyList != null)
            {
                result.Companies = user.CompanyList.Select(c =>
                {
                    var CompanyViewDto = c.ToViewDto<CompanyViewDto>();

                    if (c.PermissionKeys != null)
                    {
                        CompanyViewDto.Permissions = allPermissions.Where(p => c.PermissionKeys.Contains(p.Key));
                    }

                    return CompanyViewDto;
                });
            }

            return result;
        }
    }
}
