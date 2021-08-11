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
    [Route("api/mycompany/{companyId}/users")]
    public class UsersController : UserManagementBaseController
    {
        private readonly IRole _roleRepository;
        private readonly IUser _userRepository;
        private readonly PermissionOptions _premissionOptions;
        private readonly IPermissionRepository _permissionRepository;

        public UsersController(IUser userRepository, IRole roleRepository, IUserSynchronization userSynchronization, PermissionOptions premissionOptions, IPermissionRepository permissionRepository) : base(userRepository, premissionOptions)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _premissionOptions = premissionOptions;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("")]
        [AccessibleCompanyFilter]
        public async Task<IEnumerable<UserViewModel>> GetUsers([FromRoute] string companyId)
        {
            return await GetUsersOfCompany(companyId);
        }

        [HttpGet]
        [Route("{id}")]
        [AccessibleCompanyFilter]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<UserViewModel> GetUser([FromRoute] string companyId, [FromRoute] string id)
        {
            var user = await GetUserById(id);
            user = PruneUserInfo(user, companyId);
            if (user.CompanyIdList.Contains(companyId))
                return user;

            return null;
        }


        [HttpPut]
        [Route("{id}")]
        [AccessibleCompanyFilter]
        [PermissionAuthorize(Premissions.ManageUser)]
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
            roleIds = roleIds.Concat(user.RoleList.Where(t => t.CompanyId != companyId).Select(t => t.Id)).ToList();
            user.RoleIds = string.Join(';', roleIds);

            if (!user.CompanyIdList.Contains(companyId))
            {
                user.CompanyIds = user.CompanyIds + ";" + companyId;
            }

            await _userRepository.Update(user);

        }

        [HttpPost]
        [Route("")]
        [AccessibleCompanyFilter]
        [PermissionAuthorize(Premissions.ManageUser)]
        public async Task<string> CreateUser([FromRoute] string companyId, [FromBody] UserEditModel model)
        {
            var currentUser = await GetCurrentUser();
            var user = await _userRepository.ReadByIdentityId(model.VeracityId);
            var roleIds = await PruneRoles(companyId, model.RoleIds);
            if (user == null)
            {
                user = new User
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
        [AccessibleCompanyFilter]
        [PermissionAuthorize(Premissions.ManageUser)]
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
                user.RoleIds = string.Join(';', roleIds);
                user.UpdatedBy = $"{currentUser.FirstName} {currentUser.LastName}";
                await _userRepository.Update(user);
            }
        }


        [HttpGet]
        [Route("~/api/mycompany/{companyId}/users/currentUser")]
        public async Task<UserViewModel> GetCompanyUserByIdentityId([FromRoute] string companyId)
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext);
            var user = await GetUserByIdentityId(varacityId);
            user = PruneUserInfo(user, companyId);
            return user;
        }

        [HttpGet]
        [Route("~/api/mycompany/{companyId}/users/{id}/permissions")]
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
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext);
            return await GetUserByIdentityId(varacityId);
        }

        [HttpGet]
        [Route("~/api/users/{id}/permissions")]
        [PermissionAuthorize(Premissions.ViewUser)]
        public async Task<IEnumerable<string>> GetUserPermissions([FromRoute] string id)
        {
            var user = await _userRepository.Read(id);
            return user.RoleList.SelectMany(t => t.PermissionKeys);
        }

        [HttpGet]
        [Route("~/api/crosscompany/users")]
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
        [Route("~/api/crosscompany/{companyid}/users")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<IEnumerable<UserViewModel>> GetCompanyUsers([FromRoute] string companyid)
        {
            return await GetUsersOfCompany(companyid);
        }



        [HttpGet]
        [Route("~/api/crosscompany/users/{id}")]
        [PermissionAuthorize(Premissions.ViewUser, Premissions.ViewCompany)]
        public async Task<UserViewModel> GetCrossCompanyUser([FromRoute] string id)
        {
            return await GetUserById(id);
        }

        [HttpPost]
        [Route("~/api/crosscompany/users")]
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
            user = new User
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
        [Route("~/api/crosscompany/users/{id}")]
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
        [Route("~/api/crosscompany/users/{id}")]
        [PermissionAuthorize(Premissions.ManageUser, Premissions.ViewCompany)]
        public async Task DeleteCrossCompanyUser([FromRoute] string id)
        {
            await _userRepository.Delete(id);
        }

        private async Task<IList<string>> PruneRoles(string companyId, IList<string> sourceRoleIds)
        {
            var roles = await _roleRepository.GetRolesOfCompany(companyId);
            return sourceRoleIds.Where(t => roles.Any(f => f.Id == t)).ToList();
        }

        private async Task<IList<string>> PruneRoles(IList<string> companyIds, IList<string> sourceRoleIds)
        {
            var roles = new List<Role>();
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
                    dto.Roles = t.RoleList.Where(r => r.CompanyId == companyId).Select(r =>
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

        private async Task<UserViewModel> GetUserByIdentityId(string id)
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext);
            var user = await _userRepository.ReadByIdentityId(varacityId);
            return await PopulateUserInfo(user);
        }

        private async Task<UserViewModel> GetUserById(string userId)
        {
            var user = await _userRepository.Read(userId);
            return await PopulateUserInfo(user);
        }

        private UserViewModel PruneUserInfo(UserViewModel user,string companyId)
        {
            user.Roles = user.Roles.Where(t => t.Company.Id == companyId).ToList();
            user.Companies = user.Companies.Where(t => t.Id == companyId).ToList();
            return user;
        }

        private async Task<UserViewModel> PopulateUserInfo(User user)
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
