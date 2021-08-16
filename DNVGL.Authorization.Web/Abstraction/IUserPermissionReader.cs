using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.Authorization.Web.Abstraction
{
    /// <summary>
    /// <para>The interface define function to get a user's permissions</para>
    /// <para>It must be implemented in the project</para>
    /// </summary>
    public interface IUserPermissionReader
    {
        /// <summary>
        /// get a user's permissions
        /// </summary>
        /// <param name="veracityId">user's veracity id</param>
        /// <returns>A List of <see cref="PermissionEntity"/>.</returns>
        Task<IEnumerable<PermissionEntity>> GetPermissions(string identity);

        Task<IEnumerable<PermissionEntity>> GetPermissions(string identity,string companyId);
    }
}
