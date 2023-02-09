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

    public class CompanyRepository : CompanyRepository<Company, Role, User>
    {
        public CompanyRepository(UserManagementContext<Company, Role, User> context) : base(context)
        {

        }

    }


    public class CompanyRepository<TCompany, TRole, TUser> : ICompany<TCompany> where TRole : Role, new() where TCompany : Company, new() where TUser : User, new()
    {
        private readonly UserManagementContext<TCompany, TRole, TUser> _context;

        public CompanyRepository(UserManagementContext<TCompany, TRole, TUser> context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TCompany>> All()
        {
            return await _context.Set<TCompany>().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<TCompany> Create(TCompany company)
        {
            if (string.IsNullOrEmpty(company.Id))
            {
                company.Id = Guid.NewGuid().ToString();
            }
            company.CreatedOnUtc = DateTime.UtcNow;
            var item = (await _context.AddAsync(company)).Entity;

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task Delete(string Id)
        {
            var company = await Read(Id);
            _context.Companys.Remove(company);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TCompany>> List(IEnumerable<string> Ids)
        {
            return await _context.Companys.Where(t => Ids.Contains(t.Id)).ToListAsync();
        }

		public IQueryable<TCompany> QueryCompanys()
		{
            return _context.Companys.AsQueryable();
        }

		public async Task<TCompany> Read(string Id)
        {
            return await _context.Companys.SingleOrDefaultAsync(t => t.Id == Id);
        }

        public async Task<TCompany> ReadByDomain(string domain)
        {
            return await _context.Companys.SingleOrDefaultAsync(t => t.DomainUrl == domain);
        }

        public async Task Update(TCompany company)
        {
            company.UpdatedOnUtc = DateTime.UtcNow;
            _context.Companys.Update(company);
            await _context.SaveChangesAsync();
        }
    }
}
