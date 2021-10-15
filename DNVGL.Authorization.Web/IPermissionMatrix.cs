// Copyright (c) DNV. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
