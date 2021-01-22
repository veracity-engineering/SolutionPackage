using System;
using System.Threading.Tasks;
using Xunit;
using static DNVGL.Authorization.Web.PermissionMatrix;

namespace DNVGL.Authorization.Web.Tests
{
    public class PermissionUnitTest
    {
        [Fact]
        public void TestPermissionValueAttribute()
        {
            var expectedKey = "ManageUser";
            var key = Premissions.ManageUser.GetPermissionKey();
            Assert.Equal(expectedKey, key);
        }

        [Fact]
        public async Task TestPermissionRepositoryAsync()
        {
            var repo = new PermissionRepository();
            var lst = await repo.GetAll();
            Assert.NotNull(lst);
            Assert.Contains(lst, t => t.Name == "Manage User");
        }

        [Fact]
        public void TestPermissionAuthorizeAttribute()
        {
            var attribute = new PermissionAuthorizeAttribute(Premissions.ManageRole, Premissions.ManageUser, Premissions.ViewRole, Premissions.ViewUser);
            Assert.Equal(4, attribute.PermissionsToCheck.Length);
        }

        [Fact]
        public async Task TestCustomizedPermissionsAsync()
        {
            var repo = new PermissionRepository();
            var lst = await repo.GetAll();
            Assert.NotNull(lst);
            Assert.Contains(lst, t => t.Name == "MockUser");
        }
    }
}
