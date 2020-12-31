using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class UserRepository : IUser
    {
        public Task<IEnumerable<User>> All()
        {
            throw new NotImplementedException();
        }

        public Task<User> Create(User user)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersOfCompany(string companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersOfRole(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<User> Read(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
