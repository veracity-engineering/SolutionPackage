using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IAggregateRoot
    {
	    Task<T> GetAsync(string id, CancellationToken cancellationToken = default);

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
