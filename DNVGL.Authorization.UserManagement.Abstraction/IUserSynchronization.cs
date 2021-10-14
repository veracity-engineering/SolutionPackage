using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface IUserSynchronization<T> where T : User
    {
        Task<T> SyncUser(T user);
        void LaunchMonitoring();
        Task StopMonitoring();
    }
}
