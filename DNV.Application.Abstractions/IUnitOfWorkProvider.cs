using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWorkProvider
    {        
        IReadOnlyCollection<Entity> ChangedEntities{ get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
