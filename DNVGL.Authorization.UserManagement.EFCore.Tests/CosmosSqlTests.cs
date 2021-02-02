using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DNVGL.Authorization.UserManagement.EFCore.Tests
{
    public class CosmosSqlTests
    {
        private static UserManagementContext CreateContext(DbContextOptions<UserManagementContext> options, Action<ModelBuilder> buildModel) => new UserManagementContext(options, buildModel);

        public CosmosSqlTests()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                     .UseCosmos("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", databaseName: "UserManagement")
                     .Options;

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }


        [Fact]
        public async Task CreateRoleAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                .UseCosmos("https://localhost:8081","C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",databaseName: "UserManagement")
                .Options;


            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var roleRepository = new RoleRepository(context);
                var roleAdded = await roleRepository.Create(new Role()
                {
                    Id = "2",
                    Name = "Admin",
                    Description = "Administrator",
                    Active = true,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    Permissions = "ManageUser;ViewUser"
                });

            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                Assert.Equal("2", context.Roles.Find("2").Id);
            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var roleRepository = new RoleRepository(context);
                await roleRepository.Delete("2");
            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var roleRepository = new RoleRepository(context);
                var role = await roleRepository.Read("2");
                Assert.Null(role);
            }
        }

        [Fact]
        public async Task CreateUserAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
              .UseCosmos("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", databaseName: "UserManagement")
              .Options;


            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var roleRepository = new RoleRepository(context);
                var roleAdded = await roleRepository.Create(new Role()
                {
                    Id = "1",
                    Name = "Admin",
                    Description = "Administrator",
                    Active = true,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    Permissions = "ManageUser;ViewUser"
                });


                var companyRepository = new CompanyRepository(context);
                var company = await companyRepository.Create(new Company()
                {
                    Id = "1",
                    Active = true,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    Name = "Company A",
                    Deleted = false,
                    Description = "Company A",
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow
                });

                var userRepository = new UserRepository(context);
                var user = await userRepository.Create(new User()
                {
                    Id = "1",
                    Active = true,
                    Company = company,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    Description = "DNVGL User",
                    Email = "He.Ke.Henry.Zhang@Dnvgl.com",
                    FirstName = "Zhang",
                    LastName = "Henry",
                    Role = roleAdded,
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow,
                    VeracityId = "aba"
                });

            }


            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                Assert.Equal("1", context.Users.Find("1").Id);

                var userRepository = new UserRepository(context);

                var users = await userRepository.GetUsersOfCompany("1");
                Assert.Equal("1", users.First().Id);

                users = await userRepository.GetUsersOfRole("1");
                Assert.Equal("1", users.First().Id);

                var user = await userRepository.Read("1");
                Assert.Equal("1", user.Id);
                user.Description = "DNV";
                await userRepository.Update(user);
            }


            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var userRepository = new UserRepository(context);
                var companyRepository = new CompanyRepository(context);
                var roleRepository = new RoleRepository(context);
                var user = await userRepository.Read("1");
                Assert.Equal("DNV", user.Description);

                await userRepository.Delete("1");
                await roleRepository.Delete("1");
                await companyRepository.Delete("1");
            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var userRepository = new UserRepository(context);
                var companyRepository = new CompanyRepository(context);
                var roleRepository = new RoleRepository(context);

                var role = await roleRepository.Read("1");
                var company = await companyRepository.Read("1");
                var user = await userRepository.Read("1");
                Assert.Null(role);
                Assert.Null(company);
                Assert.Null(user);
            }

        }
    }
}
