using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.This.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.This.Abstractions
{
    public interface IThisUsers
    {
        Task<IEnumerable<UserReference>> Resolve(string email);

        Task<CreateUserReference> Create(CreateUserOptions options);

        Task<IEnumerable<CreateUserReference>> Create(params CreateUserOptions[] options);
    }
}
