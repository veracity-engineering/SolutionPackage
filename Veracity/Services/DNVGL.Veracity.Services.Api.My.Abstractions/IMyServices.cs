using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
    public interface IMyServices
    {
		/// <summary>
		/// Returns a collection of services the authenticated user is subscribed to.
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<MyServiceReference>> List();
    }
}
