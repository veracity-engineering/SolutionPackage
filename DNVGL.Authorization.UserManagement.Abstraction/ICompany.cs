using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.Abstraction.Entity;

namespace DNVGL.Authorization.UserManagement.Abstraction
{
    public interface ICompany<T> where T: Company
    {
        Task<T> Create(T company);
        Task<T> Read(string Id);
        Task<T> ReadByDomain(string domain);
        Task<IEnumerable<T>> List(IEnumerable<string> Ids);
        Task Update(T company);
        Task Delete(string Id);
        Task<IEnumerable<T>> All();
    }
}
