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
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class UserManagementBaseController<TUser> : ControllerBase where TUser : User, new()
    {
        private readonly PermissionOptions _premissionOptions;
        private readonly IUser<TUser> _userRepository;

        protected UserManagementBaseController(IUser<TUser> userRepository, PermissionOptions premissionOptions)
        {
            _userRepository = userRepository;
            _premissionOptions = premissionOptions;
        }

        protected async Task<TUser> GetCurrentUser()
        {
            var varacityId = _premissionOptions.GetUserIdentity(HttpContext.User);
            return await _userRepository.ReadByIdentityId(varacityId);
        }

        protected async Task<bool> IsCompanyAccessible(string companyId)
        {
            var user = await GetCurrentUser();
            return user.CompanyIdList.Contains(companyId);
        }

        protected async Task<RoleViewDto> FetchRole<TRole>(string id, IPermissionRepository permissionRepository, IRole<TRole> roleRepository) where TRole : Role, new()
        {
            var role = await roleRepository.Read(id);
            var allPermissions = await permissionRepository.GetAll();
            var result = role.ToViewDto<RoleViewDto>();
            result.permissions = allPermissions.Where(p => role.PermissionKeys.Contains(p.Key));

            return result;
        }

        protected UserViewModel PruneUserInfo(UserViewModel user, string companyId)
        {
            user.Roles = user.Roles.Where(t => t.Company.Id == companyId).ToList();
            user.Companies = user.Companies.Where(t => t.Id == companyId).ToList();
            return user;
        }

        protected UserViewModel PruneUserCompanyInfo(UserViewModel user, string companyId)
        {
            user.Companies = user.Companies.Where(t => t.Id == companyId).ToList();
            return user;
        }

        protected UserViewModel PopulateUserRoleInfo(TUser user, UserViewModel userViewModel,IEnumerable<PermissionEntity> allPermissions)
        {
            if (user.RoleList != null)
            {
                userViewModel.Roles = user.RoleList.Select(r =>
                {
                    var RoleViewDto = r.ToViewDto<RoleViewDto>();

                    if (r.PermissionKeys != null)
                    {
                        RoleViewDto.permissions = allPermissions.Where(p => r.PermissionKeys.Contains(p.Key));
                    }

                    return RoleViewDto;
                });
            }

            return userViewModel;
        }

        protected async Task<IEnumerable<UserViewModel>> GetAllUsers(IUser<TUser> userRepository, IPermissionRepository permissionRepository)
        {
            var users = await userRepository.All();
            var allPermissions = await permissionRepository.GetAll();

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

        protected async Task<IEnumerable<RoleViewDto>> GetAllRoles<TRole>(IRole<TRole> roleRepository, IPermissionRepository permissionRepository) where TRole : Role, new()
        {
            var roles = await roleRepository.All();
            var allPermissions = await permissionRepository.GetAll();

            var result = roles.Select(t =>
            {
                var dto = t.ToViewDto<RoleViewDto>();

                if (t.PermissionKeys != null)
                {
                    dto.permissions = allPermissions.Where(p => t.PermissionKeys.Contains(p.Key));
                }

                return dto;
            });

            return result;
        }

    }
}
