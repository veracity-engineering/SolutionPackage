using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public bool HardDelete { get; set; }

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

            if (HardDelete == false)
            {
                modelBuilder.Entity<TCompany>().HasQueryFilter(m => EF.Property<bool>(m, "Deleted") == false);
                modelBuilder.Entity<TRole>().HasQueryFilter(m => EF.Property<bool>(m, "Deleted") == false);
                modelBuilder.Entity<TUser>().HasQueryFilter(m => EF.Property<bool>(m, "Deleted") == false);
            }
        }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            if (HardDelete == false)
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.CurrentValues["Deleted"] = false;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.CurrentValues["Deleted"] = true;
                            break;
                    }
                }
            }
        }

    }
}
