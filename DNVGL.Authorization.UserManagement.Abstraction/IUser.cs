using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface IUser
    {
        Task<User> Create(User user);
        Task<User> Read(string Id);
        Task Update(User user);
        Task Delete(string Id);
        Task<IEnumerable<User>> All();
        Task<IEnumerable<User>> GetUsersOfRole(string roleId);
        Task<IEnumerable<User>> GetUsersOfCompany(string companyId);
    }
}
