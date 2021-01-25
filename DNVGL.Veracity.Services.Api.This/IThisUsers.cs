using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This
{
    public interface IThisUsers
    {
        Task<IEnumerable<UserReference>> ResolveUser(string email);

        Task<CreateUserReference> CreateUser(CreateUserOptions options);

        Task<IEnumerable<CreateUserReference>> CreateUsers(params CreateUserOptions[] options);
    }
}
