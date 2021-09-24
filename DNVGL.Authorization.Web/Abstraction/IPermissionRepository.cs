using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.Authorization.Web.Abstraction
{
    /// <summary>
    /// <para>The repository interface to fetch all permissions.</para>
    /// <para>Implement this intrefact only if permissions are not defined in source code</para>
    /// <para>Implement <see cref="IPermissionMatrix"/> to define permissions in source code</para>
    /// </summary>
    public interface IPermissionRepository
    {
        /// <summary>
        /// Return all permissions
        /// </summary>
        /// <returns>A List of <see cref="PermissionEntity"/>.</returns>
        Task<IEnumerable<PermissionEntity>> GetAll();
    }
}
