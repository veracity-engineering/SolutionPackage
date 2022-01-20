using System;
using System.Threading;
using System.Threading.Tasks;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        bool AutoCommit { get; set; }

        Task<int> SaveAllEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
