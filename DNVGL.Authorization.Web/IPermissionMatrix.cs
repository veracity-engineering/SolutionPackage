using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.Web
{
    /// <summary>
    /// Marker interface indicate class which has permission defined.
    /// <para>define permission in Enum which field has attribute <see cref="PermissionValueAttribute"/> in the implemented class. Those permissions will be loaded by <see cref="PermissionRepository"/></para>
    /// </summary>
    public interface IPermissionMatrix
    {
    }
}
