using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface IUser<T> where T : User
    {
        Task<T> Create(T user);
        Task<T> Read(string Id);
        Task<T> ReadByIdentityId(string IdentityId);
        Task Update(T user);
        Task Delete(string Id);
        Task<IEnumerable<T>> All();
        Task<IEnumerable<T>> GetUsersOfRole(string roleId);
        Task<IEnumerable<T>> GetUsersOfCompany(string companyId);
    }
}
