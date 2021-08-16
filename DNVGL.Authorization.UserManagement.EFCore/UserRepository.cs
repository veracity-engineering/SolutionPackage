﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{
    public class UserRepository : IUser
    {
        private readonly UserManagementContext _context;

        public UserRepository(UserManagementContext context)
        {
            _context = context;
        }

        private async Task FetchRoleForUsers(List<User> users)
        {
            var roles =  await _context.Set<Role>().ToListAsync();
            users.ForEach(t => t.RoleList = roles.Where(r => t.RoleIdList!=null && t.RoleIdList.Contains(r.Id)).ToList());
        }

        public async Task<IEnumerable<User>> All()
        {

            var users = await _context.Set<User>().ToListAsync();
            await FetchRoleForUsers(users);
            return users;
        }

        public async Task<User> Create(User user)
        {

            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = Guid.NewGuid().ToString();
            }
            user.CreatedOnUtc = DateTime.UtcNow;
            var item = (await _context.AddAsync(user)).Entity;

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task Delete(string Id)
        {
            var user = await Read(Id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersOfCompany(string companyId)
        {
            var users = await _context.Users.Where(t => t.CompanyIds.Contains(companyId)).ToListAsync();

            await FetchRoleForUsers(users);
            return users;
        }

        public async Task<IEnumerable<User>> GetUsersOfRole(string roleId)
        {
            var users = await _context.Users.Where(t => t.RoleIds.Contains(roleId)).ToListAsync();

            await FetchRoleForUsers(users);
            return users;
        }

        public async Task<User> Read(string Id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(p => p.Id == Id);

            if (user == null)
                return null;

            var companys = await _context.Companys.Where(t => user.CompanyIdList.Contains(t.Id)).ToListAsync();
            var roles = await _context.Roles.Where(r => user.CompanyIdList.Contains(r.CompanyId)).ToListAsync();
            user.CompanyList = companys;
            user.RoleList = roles;


            return user;
        }

        public async Task<User> ReadByIdentityId(string IdentityId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(p => p.VeracityId == IdentityId);

            if (user == null)
                return null;

            var companys = await _context.Companys.Where(t => user.CompanyIdList.Contains(t.Id)).ToListAsync();
            var roles = await _context.Roles.Where(r => user.CompanyIdList.Contains(r.CompanyId)).ToListAsync();
            user.CompanyList = companys;
            user.RoleList = roles;


            return user;
        }

        public async Task Update(User user)
        {
            user.UpdatedOnUtc = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
