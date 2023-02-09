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

    public class RoleRepository : RoleRepository<Company, Role, User>
    {
        public RoleRepository(UserManagementContext<Company, Role, User> context) : base(context)
        {

        }

    }

    public class RoleRepository<TCompany, TRole, TUser> : IRole<TRole> where TRole : Role, new() where TCompany : Company, new() where TUser : User, new()
    {
        private readonly UserManagementContext<TCompany, TRole, TUser> _context;

        public RoleRepository(UserManagementContext<TCompany, TRole, TUser> context)
        {
            _context = context;
        }


        private async Task FetchCompanyForRoles(List<TRole> roles)
        {
            var companys = await _context.Set<Company>().ToListAsync();
            roles.ForEach(t => t.Company = companys.Find(f => f.Id == t.CompanyId));
        }



        public async Task<IEnumerable<TRole>> All()
        {
            var roles = await _context.Set<TRole>().OrderBy(t => t.Name).ToListAsync();
            await FetchCompanyForRoles(roles);
            return roles;
        }

        public async Task<TRole> Create(TRole role)
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

        public async Task<IEnumerable<TRole>> GetRolesOfCompany(string companyId)
        {
            var roles = await _context.Roles.Where(t => t.CompanyId == companyId).ToListAsync();
            await FetchCompanyForRoles(roles);
            return roles;
        }

        public async Task<TRole> Read(string Id)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(t => t.Id == Id);

            if (role != null)
                role.Company = await _context.Companys.SingleOrDefaultAsync(t => t.Id == role.CompanyId);
            return role;
        }

        public async Task Update(TRole role)
        {
            role.UpdatedOnUtc = DateTime.UtcNow;
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

		public IQueryable<TRole> QueryRoles()
		{
            return _context.Roles.AsQueryable();
        }
    }
}
