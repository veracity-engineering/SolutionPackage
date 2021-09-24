using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.Web.Tests.Mock
{
    public class MockPermissionMatrix: IPermissionMatrix
    {
        public enum MockPremissions
        {
            [PermissionValue(id: "5", key: "MockUser", name: "MockUser", group: "Admin", description: "MockUser")]
            MockUser,
        }
    }
}
