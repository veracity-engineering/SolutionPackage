using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DNV.Application.Abstractions.UoW
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitOfWork: IUnitOfWork
    {
        private readonly IUoWProvider _uowContext;

        private int _disposed;

        public bool AutoCommit { get; set; }

        internal UnitOfWork(IUoWProvider uowContext, bool autoCommit)
        {
            _uowContext = uowContext ?? throw new ArgumentNullException(nameof(uowContext));
            AutoCommit = autoCommit;
        }

        public virtual async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_uowContext.ChangedEntities.Count <= 0)
                return true;

            await FlushDomainEventsAsync(_uowContext.EventHub, _uowContext);

            var result = await _uowContext.SaveChangesAsync(cancellationToken);

            return result >= 0;
        }

        private static async Task FlushDomainEventsAsync(IEventHub eventHub, IUoWProvider uowContext)
        {
	        var entities = uowContext.ChangedEntities.Where(e => e.DomainEvents?.Any() ?? false).ToList();

	        var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

	        entities.ForEach(e => e.ClearDomainEvents());

	        var tasks = domainEvents.Select(e => eventHub.PublishAsync(e));

	        await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

                if (AutoCommit)
                {
                    if (!SaveEntitiesAsync().Result)
                        throw new ApplicationException($"Failed to save changed entities. +(UnitOfWork provider type: '{_uowContext.GetType().FullName}') ");
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (Interlocked.Exchange(ref _disposed, _disposed) == 1)
                throw new ObjectDisposedException($"{nameof(UnitOfWork)} has been disposed.");
        }
    }
}
