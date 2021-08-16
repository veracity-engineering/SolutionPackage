using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class UserManagementContext : DbContext
    {

        public DbSet<Role> Roles { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<User> Users { get; set; }
        public Action<ModelBuilder> PrebuildModel { get; set; }
        public UserManagementContext(DbContextOptions<UserManagementContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (PrebuildModel != null)
            {
                PrebuildModel(modelBuilder);
            }

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(t => t.PermissionKeys);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(t => t.PermissionKeys);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(t => t.RoleIdList);
                entity.Ignore(t => t.RoleList);
                entity.Ignore(t => t.CompanyIdList);
                entity.Ignore(t => t.CompanyList);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
