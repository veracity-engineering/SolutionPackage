using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Directory.Abstractions
{
	public interface ICompanyDirectory
	{
		/// <summary>
		/// Returns the company by company id.
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		Task<Company> Get(string companyId);

		/// <summary>
		/// Returns a collection of users affiliated with company by company id.
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<IEnumerable<UserReference>> ListUsers(string companyId, int page = 1, int pageSize = 20);
	}
}
