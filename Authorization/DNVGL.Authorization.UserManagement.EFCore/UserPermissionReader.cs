using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class UserPermissionReader : UserPermissionReader<Company, Role, User>
    {
        public UserPermissionReader(UserManagementContext<Company, Role, User> context, IPermissionRepository permissionRepository, UserManagementSettings userManagementSettings) : base(context, permissionRepository, userManagementSettings)
        {

        }

    }

    public class UserPermissionReader<TCompany, TRole, TUser> : IUserPermissionReader where TRole : Role, new() where TCompany : Company, new() where TUser : User, new()
    {
        private readonly UserManagementContext<TCompany, TRole, TUser> _context;
        private readonly IPermissionRepository _permissionRepository;
        private readonly UserManagementSettings _userManagementSettings;
        public UserPermissionReader(UserManagementContext<TCompany, TRole, TUser> context, IPermissionRepository permissionRepository, UserManagementSettings userManagementSettings)
        {
            _context = context;
            _permissionRepository = permissionRepository;
            _userManagementSettings = userManagementSettings;
        }

        public async Task<IEnumerable<PermissionEntity>> GetPermissions(string identity)
        {
            return await GetPermissions(identity, null);
        }

        public async Task<IEnumerable<PermissionEntity>> GetPermissions(string identity, string companyId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(p => p.VeracityId.ToLower() == identity.ToLower() || p.Id == identity);

            if (user == null)
                return null;

            var allPermissions = (await _permissionRepository.GetAll());

            if (user.SuperAdmin)
            {
                return allPermissions;
            }

            if (string.IsNullOrEmpty(user.RoleIds))
                return null;

            var role = await _context.Roles.Where(t => user.RoleIds.Contains(t.Id)).ToListAsync();

            if (!string.IsNullOrEmpty(companyId) && _userManagementSettings.Mode == UserManagementMode.Company_CompanyRole_User)
                role = role.Where(t => t.CompanyId == companyId).ToList();

            var allAssignedPermissions = role.SelectMany(t => t.PermissionKeys);

            if (allAssignedPermissions.Any())
            {
                return allPermissions.Where(p => allAssignedPermissions.Contains(p.Key) || allAssignedPermissions.Contains(p.Id));
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<PermissionEntity>> GetPermissions(IEnumerable<string> permissions)
        {
            var allPermissions = (await _permissionRepository.GetAll());

            if (permissions.Any())
            {
                return allPermissions.Where(p => permissions.Contains(p.Key) || permissions.Contains(p.Id));
            }
            else
            {
                return null;
            }
        }
    }
}
