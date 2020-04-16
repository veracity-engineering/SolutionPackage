using System;
using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        bool AutoCommit { get; set; }

        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
