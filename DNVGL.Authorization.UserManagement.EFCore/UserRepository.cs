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

        public async Task<IEnumerable<User>> All()
        {
            return await _context.Set<User>().Include(b => b.Company).Include(b => b.Role).ToListAsync();
        }

        public async Task<User> Create(User user)
        {
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
            return await _context.Users.Include(t => t.Company).Include(t => t.Role).Where(t => t.CompanyId == companyId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersOfRole(string roleId)
        {
            return await _context.Users.Include(t => t.Company).Include(t => t.Role).Where(t => t.RoleId == roleId).ToListAsync();
        }

        public async Task<User> Read(string Id)
        {
            return await _context.Users.Include(b => b.Company).Include(b => b.Role).SingleOrDefaultAsync(p => p.Id == Id);
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
