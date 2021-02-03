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
            var user = await _context.Users.SingleOrDefaultAsync(p => p.Id == identity);

            if (user == null || string.IsNullOrEmpty(user.RoleId))
                return null;

            var role = await _context.Roles.SingleOrDefaultAsync(t => t.Id == user.RoleId);

            if (role.PermissionKeys.Any())
            {
                var allPermissions = (await _permissionRepository.GetAll());
                return allPermissions.Where(p => role.PermissionKeys.Contains(p.Key) || role.PermissionKeys.Contains(p.Id));
            }
            else
            {
                return null;
            }
        }
    }
}
