using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface IRole
    {
        Task<Role> Create(Role role);
        Task<Role> Read(string Id);
        Task Update(Role role);
        Task Delete(string Id);
        Task<IEnumerable<Role>> All();
    }
}
