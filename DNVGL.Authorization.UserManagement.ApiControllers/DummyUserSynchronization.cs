using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    public class DummyUserSynchronization : IUserSynchronization<User>
    {
        public Task<User> PullUserFromSource(User user)
        {
            return (Task<User>)Task.CompletedTask;
        }

        public Task<User> PushUserToSource(User user)
        {
            return (Task<User>)Task.CompletedTask;
        }

        public Task<User> RemoveUserAtSource(User user)
        {
            return (Task<User>)Task.CompletedTask;
        }

        public Task<User> RemoveUserAtLocal(User user)
        {
            return (Task<User>)Task.CompletedTask;
        }

    }
}
