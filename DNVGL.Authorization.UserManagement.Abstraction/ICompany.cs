using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface ICompany
    {
        Task<Company> Create(Company company);
        Task<Company> Read(string Id);
        Task<Company> Update(Company company);
        Task Delete(string Id);
        Task<IEnumerable<Company>> All();
    }
}
