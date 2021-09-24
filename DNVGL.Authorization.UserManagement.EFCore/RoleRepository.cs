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
    public class RoleRepository : IRole<Role>
    {
        private readonly UserManagementContext _context;

        public RoleRepository(UserManagementContext context)
        {
            _context = context;
        }


        private async Task FetchCompanyForRoles(List<Role> roles)
        {
            var companys = await _context.Set<Company>().ToListAsync();
            roles.ForEach(t => t.Company = companys.Find(f=>f.Id==t.CompanyId));
        }



        public async Task<IEnumerable<Role>> All()
        {
            var roles = await _context.Set<Role>().OrderBy(t => t.Name).ToListAsync();
            await FetchCompanyForRoles(roles);
            return roles;
        }

        public async Task<Role> Create(Role role)
        {
            if (string.IsNullOrEmpty(role.Id))
            {
                role.Id = Guid.NewGuid().ToString();
            }
            role.CreatedOnUtc = DateTime.UtcNow;
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

        public async Task<IEnumerable<Role>> GetRolesOfCompany(string companyId)
        {
            var roles = await _context.Roles.Where(t => t.CompanyId == companyId).ToListAsync();
            await FetchCompanyForRoles(roles);
            return roles;
        }

        public async Task<Role> Read(string Id)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(t => t.Id == Id);

            if (role != null)
                role.Company = await _context.Companys.SingleOrDefaultAsync(t => t.Id == role.CompanyId);
            return role;
        }

        public async Task Update(Role role)
        {
            role.UpdatedOnUtc = DateTime.UtcNow;
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }
    }
}
