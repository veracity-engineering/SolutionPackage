using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.Web.Tests.Mock
{
    public class MockPermissionMatrix: IPermissionMatrix
    {
        public enum MockPremissions
        {
            [PermissionValue(Id = "5", Key = "MockUser", Name = "MockUser", Group = "Admin", Description = "MockUser")]
            MockUser,
        }
    }
}
