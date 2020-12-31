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
    public class CompanyRepository : ICompany
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

        public async Task<Company> Read(string Id)
        {
            return await _context.Companys.FindAsync(Id);
        }

        public async Task Update(Company company)
        {
            _context.Companys.Update(company);
            await _context.SaveChangesAsync();
        }
    }
}
