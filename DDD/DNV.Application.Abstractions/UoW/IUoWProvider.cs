using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions.UoW
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUoWProvider
    {
	    void JoinUoW<T>(IRepository<T> repository) where T : Entity, IAggregateRoot;

        IReadOnlyCollection<Entity> ChangedEntities{ get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
