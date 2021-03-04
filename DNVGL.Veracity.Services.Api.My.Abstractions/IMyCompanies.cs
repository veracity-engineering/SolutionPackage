using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
	public interface IMyCompanies
	{
		/// <summary>
		/// Returns a collection of companies the authenticated user is affilated with.
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<CompanyReference>> List();
	}
}
