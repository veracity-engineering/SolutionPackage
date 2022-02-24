using System;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
	    TR? ResolveRepository<TR, TE>() where TR : class, IRepository<TE> where TE : Entity, IAggregateRoot;

        bool AutoCommit { get; set; }

        Task<int> SaveAllEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
