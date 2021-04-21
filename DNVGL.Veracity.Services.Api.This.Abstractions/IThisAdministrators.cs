using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.Abstractions
{
	public interface IThisAdministrators
	{
		/// <summary>
		/// Returns that administrator of the authenticated company by user id.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<Administrator> Get(string userId);

		/// <summary>
		/// Returns a collection of administrators for the authenticated company.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<IEnumerable<AdministratorReference>> List(int page, int pageSize);
	}
}
