using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.Domain.Seedwork
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
