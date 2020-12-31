using System;
using System.Linq;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DNVGL.Authorization.UserManagement.EFCore.Tests
{
    public class InMemoryTests
    {
        private static UserManagementContext CreateContext(DbContextOptions<UserManagementContext> options)
           => new UserManagementContext(options);
        private Role ExpectedRole = new Role() { Id = "1", Name = "Admin",Permissions= "ManageUser;ViewUser" };


        [Fact]
        public async Task CreateRoleAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                .UseInMemoryDatabase(databaseName: "CreateRole")
                .Options;

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                var roleAdded = await roleRepository.Create(new Role() { Id="1"});

            }

            using (var context = CreateContext(options))
            {
                Assert.Equal(1, context.Roles.Count());
                Assert.Equal("1", context.Roles.Single().Id);
            }

        }

        [Fact]
        public async Task GetAllRoleAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                .UseInMemoryDatabase(databaseName: "GetAllAsync")
                .Options;

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                var roleAdded = await roleRepository.Create(ExpectedRole);
                var roleList = await roleRepository.All();

                Assert.Single(roleList);
                Assert.Contains(roleList,t=>t.PermissionKeys.Any(x=>x == "ViewUser"));
            }
        }

        [Fact]
        public async Task ReadRoleAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                           .UseInMemoryDatabase(databaseName: "ReadAsync")
                           .Options;

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                await roleRepository.Create(ExpectedRole);
            }

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                var role = await roleRepository.Read("1");
                Assert.Equal("Admin", role.Name);
            }
        }

        [Fact]
        public async Task DeleteRoleAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                           .UseInMemoryDatabase(databaseName: "DeleteAsync")
                           .Options;

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                await roleRepository.Create(ExpectedRole);
            }

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                await roleRepository.Delete("1");
            }


            using (var context = CreateContext(options))
            {
                Assert.Equal(0, context.Roles.Count());
            }
        }

        [Fact]
        public async Task UpdateRoleAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateAsync")
                           .Options;

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                await roleRepository.Create(ExpectedRole);
            }

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                var role = await roleRepository.Read("1");
                role.Permissions = role.Permissions + ";ViewRole";
                await roleRepository.Update(role);
            }


            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                var role = await roleRepository.Read("1");
                Assert.Contains(role.PermissionKeys, t => t == "ViewRole");
            }
        }

    }
}
