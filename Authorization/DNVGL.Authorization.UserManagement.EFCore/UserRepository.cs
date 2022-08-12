using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using Microsoft.EntityFrameworkCore;

namespace DNVGL.Authorization.UserManagement.EFCore
{

    public class UserRepository : UserRepository<Company, Role, User>
    {
        public UserRepository(UserManagementContext<Company, Role, User> context) : base(context)
        {

        }

    }

    public class UserRepository<TCompany, TRole, TUser> : IUser<TUser> where TRole : Role, new() where TCompany : Company, new() where TUser : User, new()
    {
        private readonly UserManagementContext<TCompany, TRole, TUser> _context;

        public UserRepository(UserManagementContext<TCompany, TRole, TUser> context)
        {
            _context = context;
        }

        private async Task FetchRoleForUsers(List<TUser> users)
        {
            var roles =  await _context.Set<TRole>().ToListAsync();
            users.ForEach(t => t.RoleList = roles.Where(r => t.RoleIdList!=null && t.RoleIdList.Contains(r.Id)).ToList());
        }

        public async Task<IEnumerable<TUser>> All(int page = 0, int size = 0)
        {
            List<TUser> users;

            if (page * size == 0)
            {
                users = await _context.Set<TUser>().ToListAsync();
            }
            else
            {
                users = await _context.Set<TUser>().Skip((page - 1) * size).Take(size).ToListAsync();
            }

     
            await FetchRoleForUsers(users);
            return users;
        }

        public async Task<TUser> Create(TUser user)
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

        public async Task<IEnumerable<TUser>> GetUsersOfCompany(string companyId, int page = 0, int size = 0)
        {

            List<TUser> users;

            if (page * size == 0)
            {
                users = await _context.Users.Where(t => t.CompanyIds.Contains(companyId)).ToListAsync();
            }
            else
            {
                users = await _context.Users.Where(t => t.CompanyIds.Contains(companyId)).Skip((page - 1) * size).Take(size).ToListAsync();
            }


            await FetchRoleForUsers(users);
            return users;
        }

        public async Task<IEnumerable<TUser>> GetUsersOfRole(string roleId)
        {
            var users = await _context.Users.Where(t => t.RoleIds.Contains(roleId)).ToListAsync();

            await FetchRoleForUsers(users);
            return users;
        }

        public async Task<TUser> Read(string Id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(p => p.Id == Id);

            return await FetchUserCompanyRole(user);
        }

        public async Task<TUser> ReadByIdentityId(string IdentityId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(p => p.VeracityId.ToLower() == IdentityId.ToLower());

            return await FetchUserCompanyRole(user);
        }

        public async Task Update(TUser user)
        {
            user.UpdatedOnUtc = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<TUser> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower());

            return await FetchUserCompanyRole(user);
        }

        private async Task<TUser> FetchUserCompanyRole(TUser user)
		{
            if (user == null)
                return null;

            var companys = await _context.Companys.Where(t => user.CompanyIdList.Contains(t.Id)).ToListAsync();
            var roles = await _context.Roles.Where(r => user.RoleIdList.Contains(r.Id)).ToListAsync();
            user.CompanyList = companys;
            user.RoleList = roles;
            return user;
        }

		public IQueryable<TUser> QueryUsers()
		{
            return _context.Users.AsQueryable();
        }
	}
}
