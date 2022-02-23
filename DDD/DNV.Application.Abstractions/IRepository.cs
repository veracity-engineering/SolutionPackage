using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : Entity, IAggregateRoot
    {
	    Task<T?> GetAsync(string id, CancellationToken cancellationToken = default);

	    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predict, CancellationToken cancellationToken = default);

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
