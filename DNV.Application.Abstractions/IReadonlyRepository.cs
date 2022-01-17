using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Common.Core.Pagination;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadonlyRepository<T> where T : IAggregateRoot
    {
        Task<T> GetAsync(string id, CancellationToken cancellationToken = default);

        Task<IPaginatedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, ContinuationToken continuationToken = default, int pageSize = default, CancellationToken cancellationToken = default);
    }
}