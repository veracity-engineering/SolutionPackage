using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.Abstractions
{
	// TODO: Verify documentation
	public interface IThisUsers
	{
		/// <summary>
		/// Create and affiliate a user with the authenticated company.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<CreateUserReference> Create(CreateUserOptions options);

		/// <summary>
		/// Create and affiliate a collection of users with the authenticated company.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<IEnumerable<CreateUserReference>> Create(params CreateUserOptions[] options);

		/// <summary>
		/// Returns a user affiliated with the authenticated company by email.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		Task<IEnumerable<UserReference>> Resolve(string email);
	}
}
