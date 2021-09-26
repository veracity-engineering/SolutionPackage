using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class UserManagementContext : UserManagementContext<Company, Role, User>
    {
        public UserManagementContext(DbContextOptions options)
            : base(options)
        {
        }

    }

    public class UserManagementContext<TUser> : UserManagementContext<Company, Role, TUser> where TUser : User
    {
        public UserManagementContext(DbContextOptions options)
            : base(options)
        {
        }

    }

    public class UserManagementContext<TCompany,TUser> : UserManagementContext<TCompany, Role, TUser> where TCompany : Company where TUser : User
    {
        public UserManagementContext(DbContextOptions options)
            : base(options)
        {
        }

    }

    public class UserManagementContext<TCompany, TRole, TUser> : DbContext where TCompany : Company where TRole : Role where TUser : User
    {
        public DbSet<TRole> Roles { get; set; }
        public DbSet<TCompany> Companys { get; set; }
        public DbSet<TUser> Users { get; set; }

        public Action<ModelBuilder> PrebuildModel { get; set; }

        public UserManagementContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (PrebuildModel != null)
            {
                PrebuildModel(modelBuilder);
            }

            modelBuilder.Entity<TCompany>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(t => t.PermissionKeys);
            });

            modelBuilder.Entity<TRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(t => t.PermissionKeys);
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(t => t.RoleIdList);
                entity.Ignore(t => t.RoleList);
                entity.Ignore(t => t.CompanyIdList);
                entity.Ignore(t => t.CompanyList);
            });
        }
    }
}
