using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace DNVGL.Authorization.UserManagement.EFCore.Tests
{
    public class MSSQLTests
    {
#if DEBUG
        private const string CONNECTION_STRING = @"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;";
        private static UserManagementContext CreateContext(DbContextOptions<UserManagementContext> options) => new UserManagementContext(options);


        public MSSQLTests()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>().UseSqlServer(CONNECTION_STRING).Options;

            using (var context = CreateContext(options))
            {
                RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();

                if (!databaseCreator.HasTables())
                {
                    databaseCreator.CreateTables();
                }
            }
        }

        [Fact]
        public async Task CreateRoleAsync()
        {
            var options = new DbContextOptionsBuilder<UserManagementContext>()
                .UseSqlServer(CONNECTION_STRING)
                .Options;


            using (var context = CreateContext(options))
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

            using (var context = CreateContext(options))
            {
                Assert.Equal("ut3", context.Roles.Find("ut3").Id);
            }

            using (var context = CreateContext(options))
            {
                var roleRepository = new RoleRepository(context);
                await roleRepository.Delete("ut3");

                var companyRepository = new CompanyRepository(context);
                await companyRepository.Delete("ut2");
            }

            using (var context = CreateContext(options))
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
              .UseSqlServer(CONNECTION_STRING)
              .Options;

            using (var context = CreateContext(options))
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
                    CompanyId = company.Id,
                    Permissions = "ViewRole;ManageRole;ViewCompany;ManageCompany"
                });




                //var userRepository = new UserRepository(context);
                //var user = await userRepository.Create(new User()
                //{
                //    Id = "1",
                //    Active = true,
                //    Company = company,
                //    CreatedBy = "system",
                //    CreatedOnUtc = DateTime.UtcNow,
                //    Deleted = false,
                //    Description = "DNVGL User",
                //    Email = "He.Ke.Henry.Zhang@Dnvgl.com",
                //    FirstName = "Zhang",
                //    LastName = "Henry",
                //    Role = roleAdded,
                //    UpdatedBy = "system",
                //    UpdatedOnUtc = DateTime.UtcNow,
                //    VeracityId = "aba"
                //});


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
                    RoleIds = "ut1;ut2",
                    UpdatedBy = "system",
                    UpdatedOnUtc = DateTime.UtcNow,
                    VeracityId = "aba"
                });


            }


            using (var context = CreateContext(options))
            {
                //Assert.Equal(1, context.Users.Count());
                Assert.Equal("ut1", context.Users.Find("ut1").Id);

                var userRepository = new UserRepository(context);

                var users = await userRepository.GetUsersOfCompany("ut1");
                Assert.Equal("ut1", users.First().Id);

                users = await userRepository.GetUsersOfRole("ut1");
                Assert.Equal("ut1", users.First().Id);

                users = await userRepository.GetUsersOfRole("ut2");
                Assert.Equal("ut1", users.First().Id);

                var user = await userRepository.Read("ut1");
                Assert.Equal("ut1", user.Id);
                user.Description = "DNV";
                await userRepository.Update(user);
            }


            using (var context = CreateContext(options))
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

            using (var context = CreateContext(options))
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
