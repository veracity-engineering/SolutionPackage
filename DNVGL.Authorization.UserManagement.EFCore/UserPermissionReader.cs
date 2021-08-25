﻿using System;
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
            return await GetPermissions(identity, null);
        }

        public async Task<IEnumerable<PermissionEntity>> GetPermissions(string identity, string companyId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(p => p.VeracityId == identity || p.Id == identity);

            if (user == null || string.IsNullOrEmpty(user.RoleIds))
                return null;

            var allPermissions = (await _permissionRepository.GetAll());

            if (user.SuperAdmin)
            {
                return allPermissions;
            }

            var role = await _context.Roles.Where(t => user.RoleIds.Contains(t.Id)).ToListAsync();

            if (!string.IsNullOrEmpty(companyId))
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
