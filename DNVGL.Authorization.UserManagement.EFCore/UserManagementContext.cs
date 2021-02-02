﻿using System;
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
        private Action<ModelBuilder> _prebuildModel;
        public UserManagementContext(DbContextOptions<UserManagementContext> options)
            : base(options)
        {
        }

        public UserManagementContext(DbContextOptions<UserManagementContext> options, Action<ModelBuilder> buildModel)
       : base(options)
        {
            _prebuildModel = buildModel;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (_prebuildModel != null)
            {
                _prebuildModel(modelBuilder);
            }

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Ignore(t => t.PermissionKeys);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                //entity.HasOne<Company>().WithMany().HasForeignKey("CompanyId");
                //entity.HasOne<Role>().WithMany().HasForeignKey("RoleId");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
