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

        [HttpGet]
        [Route("")]
        [PermissionAuthorize(Premissions.ViewUser)]
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<IEnumerable<UserViewModel>> GetUsers([FromRoute] string companyId)
        {
            return await GetUsersOfCompany(companyId);
        }

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

            if (user.CompanyIdList.Contains(companyId))
                return user;

            return null;
        }


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

        [HttpGet]
        [Route("~/api/company/{companyId}/users/{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<string>> GetUserPermissions([FromRoute] string companyId, [FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return user.RoleList.Where(t => t.CompanyId == companyId).SelectMany(t => t.PermissionKeys);
        }

        [HttpGet]
        [Route("~/api/users/currentUser")]
        public async Task<UserViewModel> GetUserByIdentityId()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext.User);
            return await GetUserByIdentityId(varacityId);
        }

        [HttpGet]
        [Route("~/api/users/{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<string>> GetUserCorssCompanyPermissions([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return user.RoleList.SelectMany(t => t.PermissionKeys);
        }

        [HttpGet]
        [Route("~/api/admin/users")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<IEnumerable<UserViewModel>> GetCrossCompanyUsers()
        {
            var users = await _userRepository.All();
            var allPermissions = await _permissionRepository.GetAll();

            var result = users.Select(t =>
            {
                var dto = t.ToViewDto<UserViewModel>();

                if (t.RoleList != null)
                {
                    dto.Roles = t.RoleList.Select(r =>
                    {
                        var RoleViewDto = r.ToViewDto<RoleViewDto>();

                        if (r.PermissionKeys != null)
                        {
                            RoleViewDto.permissions = allPermissions.Where(p => r.PermissionKeys.Contains(p.Key));
                        }

                        return RoleViewDto;
                    });
                }

                return dto;
            });


            return result;
        }

        [HttpGet]
        [Route("~/api/admin/{companyid}/users")]
        [ObsoleteAttribute("It's an obsoleted end point. not suggest to use.", true)]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<IEnumerable<UserViewModel>> GetCompanyUsers([FromRoute] string companyid)
        {
            return await GetUsersOfCompany(companyid);
        }

        [HttpGet]
        [Route("~/api/admin/users/{id}")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<UserViewModel> GetCrossCompanyUser([FromRoute] string id)
        {
            return await GetUserById(id);
        }

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
            var roles = new List<TRole>();
            foreach (var companyId in companyIds)
            {
                roles.AddRange(await _roleRepository.GetRolesOfCompany(companyId));
            }
            return sourceRoleIds.Where(t => roles.Any(f => f.Id == t)).ToList();
        }

        private async Task<IEnumerable<UserViewModel>> GetUsersOfCompany(string companyId)
        {
            var users = await _userRepository.GetUsersOfCompany(companyId);
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
                                  RoleViewDto.permissions = allPermissions.Where(p => r.PermissionKeys.Contains(p.Key));
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

        private async Task<UserViewModel> PopulateUserInfo(TUser user)
        {
            var allPermissions = await _permissionRepository.GetAll();
            var result = user.ToViewDto<UserViewModel>();

            if (user.RoleList != null)
            {
                result.Roles = user.RoleList.Select(r =>
                {
                    var RoleViewDto = r.ToViewDto<RoleViewDto>();

                    if (r.PermissionKeys != null)
                    {
                        RoleViewDto.permissions = allPermissions.Where(p => r.PermissionKeys.Contains(p.Key));
                    }

                    return RoleViewDto;
                });
            }

            if (user.CompanyList != null)
            {
                result.Companies = user.CompanyList.Select(c =>
                {
                    var CompanyViewDto = c.ToViewDto<CompanyViewDto>();

                    if (c.PermissionKeys != null)
                    {
                        CompanyViewDto.permissions = allPermissions.Where(p => c.PermissionKeys.Contains(p.Key));
                    }

                    return CompanyViewDto;
                });
            }

            return result;
        }
    }
}
