using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;
using DNVGL.Common.Core.Pagination;
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

        private async Task FetchCompanyForUsers(List<TUser> users)
        {
            var companies = await _context.Set<TCompany>().ToListAsync();
            users.ForEach(t => t.CompanyList = companies.Where(r => t.CompanyIdList != null && t.CompanyIdList.Contains(r.Id)).ToList());
        }

        public async Task<PaginatedResult<TUser>> All(PageParam pageParam = null)
        {
            List<TUser> users;
            int totalCount = 0;

            if (pageParam == null)
            {
                users = await _context.Set<TUser>().ToListAsync();
                totalCount = users.Count;
            }
            else
            {
                totalCount = await _context.Users.CountAsync();
                users = await _context.Users.Skip((pageParam.PageIndex - 1) * pageParam.PageSize).Take(pageParam.PageSize).ToListAsync();
            }

     
            await FetchRoleForUsers(users);
            await FetchCompanyForUsers(users);

            var result = new PaginatedResult<TUser>(users, pageParam?.PageIndex ?? 1, pageParam?.PageSize ?? totalCount, totalCount);

            return result;
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

        public async Task<PaginatedResult<TUser>> GetUsersOfCompany(string companyId, PageParam pageParam = null)
        {

            List<TUser> users;
            int totalCount = 0;

            if (pageParam == null)
            {
                users = await _context.Users.Where(t => t.CompanyIds.Contains(companyId)).ToListAsync();
                totalCount = users.Count;
            }
            else
            {
                totalCount = await _context.Users.CountAsync(t=> t.CompanyIds.Contains(companyId));
                users = await _context.Users.Where(t => t.CompanyIds.Contains(companyId)).Skip((pageParam.PageIndex - 1) * pageParam.PageSize).Take(pageParam.PageSize).ToListAsync();
            }


            await FetchRoleForUsers(users);

            var result = new PaginatedResult<TUser>(users, pageParam?.PageIndex ?? 1, pageParam?.PageSize ?? totalCount, totalCount);

            return result;
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
