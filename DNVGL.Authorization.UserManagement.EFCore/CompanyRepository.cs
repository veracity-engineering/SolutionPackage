using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class CompanyRepository : ICompany
    {
        private readonly UserManagementContext _context;

        public CompanyRepository(UserManagementContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<Company>> All()
        {
            throw new NotImplementedException();
        }

        public Task<Company> Create(Company company)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<Company> Read(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<Company> Update(Company company)
        {
            throw new NotImplementedException();
        }
    }
}
