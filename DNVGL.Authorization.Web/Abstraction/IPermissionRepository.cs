using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.Authorization.Web.Abstraction
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<PermissionEntity>> GetAll();
    }
}
