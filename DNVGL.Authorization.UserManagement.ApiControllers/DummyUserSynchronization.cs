using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class DummyUserSynchronization : IUserSynchronization
    {
        public void LaunchMonitoring()
        {
            //throw new NotImplementedException();
        }

        public Task StopMonitoring()
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task<User> SyncUser(User user)
        {
            return Task.FromResult(user);
            //throw new NotImplementedException();
        }
    }
}
