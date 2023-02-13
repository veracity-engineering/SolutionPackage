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
#if DEBUG
        private static UserManagementContext CreateContext(DbContextOptions<UserManagementContext> options, Action<ModelBuilder> buildModel)
        {

            var db = new UserManagementContext(options);
            db.PrebuildModel = buildModel;
            return db;
        }



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

                var companyRepository = new CompanyRepository(context);
                var company = await companyRepository.Create(new Company()
                {
                    Id = "ut2",
                    Active = true,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    Name = "Company A",
                    Deleted = false,
                    Description = "Company A",
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow
                });


                var roleRepository = new RoleRepository(context);
                var roleAdded = await roleRepository.Create(new Role()
                {
                    Id = "ut3",
                    Name = "Admin",
                    Description = "Administrator",
                    Active = true,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    CompanyId = company.Id,
                    Permissions = "ManageUser;ViewUser"
                });

            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                Assert.Equal("ut3", context.Roles.Find("ut3").Id);
            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var roleRepository = new RoleRepository(context);
                await roleRepository.Delete("ut3");

                var companyRepository = new CompanyRepository(context);
                await companyRepository.Delete("ut2");
            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var roleRepository = new RoleRepository(context);
                var role = await roleRepository.Read("ut3");
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
                    Id = "ut1",
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

                var roleAdded2 = await roleRepository.Create(new Role()
                {
                    Id = "ut2",
                    Name = "Admin2",
                    Description = "Administrator",
                    Active = true,
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    Permissions = "ViewRole;ManageRole;ViewCompany;ManageCompany"
                });


                var companyRepository = new CompanyRepository(context);
                var company = await companyRepository.Create(new Company()
                {
                    Id = "ut1",
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
                    Id = "ut1",
                    Active = true,
                    CompanyIds = "ut1",
                    CreatedBy = "system",
                    CreatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    Description = "DNVGL User",
                    Email = "He.Ke.Henry.Zhang@Dnvgl.com",
                    FirstName = "Zhang",
                    LastName = "Henry",
                    RoleIds = "ut1,ut2",
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow,
                    VeracityId = "aba"
                });

            }


            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                Assert.Equal("ut1", context.Users.Find("ut1").Id);

                var userRepository = new UserRepository(context);

                var users = await userRepository.GetUsersOfCompany("ut1");
                Assert.Equal("ut1", users.First().Id);

                var users2 = await userRepository.GetUsersOfRole("ut1");
                Assert.Equal("ut1", users.First().Id);

                var users3 = await userRepository.GetUsersOfRole("ut2");
                Assert.Equal("ut1", users.First().Id);

                var user = await userRepository.Read("ut1");
                Assert.Equal("ut1", user.Id);
                user.Description = "DNV";
                await userRepository.Update(user);
            }


            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var userRepository = new UserRepository(context);
                var companyRepository = new CompanyRepository(context);
                var roleRepository = new RoleRepository(context);
                var user = await userRepository.Read("ut1");
                Assert.Equal("DNV", user.Description);

                await userRepository.Delete("ut1");
                await roleRepository.Delete("ut1");
                await roleRepository.Delete("ut2");
                await companyRepository.Delete("ut1");
            }

            using (var context = CreateContext(options, (modelBuilder) => modelBuilder.HasDefaultContainer("User")))
            {
                var userRepository = new UserRepository(context);
                var companyRepository = new CompanyRepository(context);
                var roleRepository = new RoleRepository(context);

                var role = await roleRepository.Read("ut1");
                var company = await companyRepository.Read("ut1");
                var user = await userRepository.Read("ut1");
                Assert.Null(role);
                Assert.Null(company);
                Assert.Null(user);
            }

        }
#endif
    }
}
