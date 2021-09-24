using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface IRole<T> where T : Role
    {
        Task<T> Create(T role);
        Task<T> Read(string Id);
        Task Update(T role);
        Task Delete(string Id);
        Task<IEnumerable<T>> GetRolesOfCompany(string companyId);
        Task<IEnumerable<T>> All();
    }
}
