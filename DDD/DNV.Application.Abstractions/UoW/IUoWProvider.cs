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
        IEventHub EventHub { get; }

        IReadOnlyCollection<Entity> ChangedEntities{ get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
