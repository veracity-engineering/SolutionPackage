using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class RoleRepository : IRole
    {
        private readonly UserManagementContext _context;

        public RoleRepository(UserManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> All()
        {
            return await _context.Set<Role>().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Role> Create(Role role)
        {
            if (string.IsNullOrEmpty(role.Id))
            {
                role.Id = Guid.NewGuid().ToString();
            }

            var item = (await _context.AddAsync(role)).Entity;

            await _context.SaveChangesAsync();

            return item;
        }


        public async Task Delete(string Id)
        {
            var role = await Read(Id);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<Role> Read(string Id)
        {
            return await _context.Roles.SingleOrDefaultAsync(t=>t.Id==Id);
        }

        public async Task Update(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }
    }
}
