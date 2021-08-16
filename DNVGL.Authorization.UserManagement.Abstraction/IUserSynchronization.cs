using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface IUserSynchronization
    {
        Task<User> SyncUser(User user);
        void LaunchMonitoring();
        Task StopMonitoring();
    }
}
