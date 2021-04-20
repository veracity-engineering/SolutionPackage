using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.Web;
using DNVGL.Authorization.Web.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class UserPermissionReader : IUserPermissionReader
    {
        private readonly UserManagementContext _context;
        private readonly IPermissionRepository _permissionRepository;

        public UserPermissionReader(UserManagementContext context, IPermissionRepository permissionRepository)
        {
            _context = context;
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<PermissionEntity>> GetPermissions(string identity)
        {
            var user = await _context.Users.SingleOrDefaultAsync(p => p.VeracityId == identity || p.Id == identity);

            if (user == null || string.IsNullOrEmpty(user.RoleIds))
                return null;

            var role = await _context.Roles.Where(t => user.RoleIds.Contains(t.Id)).ToListAsync();

            var allAssignedPermissions =  role.SelectMany(t => t.PermissionKeys);

            if (allAssignedPermissions.Any())
            {
                var allPermissions = (await _permissionRepository.GetAll());
                return allPermissions.Where(p => allAssignedPermissions.Contains(p.Key) || allAssignedPermissions.Contains(p.Id));
            }
            else
            {
                return null;
            }
        }
    }
}
