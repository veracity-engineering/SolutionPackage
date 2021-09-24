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
    public class CompanyRepository : ICompany<Company>
    {
        private readonly UserManagementContext _context;

        public CompanyRepository(UserManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> All()
        {
            return await _context.Set<Company>().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Company> Create(Company company)
        {
            if (string.IsNullOrEmpty(company.Id))
            {
                company.Id = Guid.NewGuid().ToString();
            }
            company.UpdatedOnUtc = DateTime.UtcNow;
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

        public async Task<IEnumerable<Company>> List(IEnumerable<string> Ids)
        {
            return await _context.Companys.Where(t => Ids.Contains(t.Id)).ToListAsync();
        }

        public async Task<Company> Read(string Id)
        {
            return await _context.Companys.SingleOrDefaultAsync(t => t.Id == Id);
        }

        public async Task<Company> ReadByDomain(string domain)
        {
            return await _context.Companys.SingleOrDefaultAsync(t => t.DomainUrl == domain);
        }

        public async Task Update(Company company)
        {
            company.UpdatedOnUtc = DateTime.UtcNow;
            _context.Companys.Update(company);
            await _context.SaveChangesAsync();
        }
    }
}
