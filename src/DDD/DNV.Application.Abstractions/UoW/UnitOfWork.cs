using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions.UoW
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitOfWork: IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUoWProvider _uowProvider;
        private readonly IEventHub? _eventHub;

        private int _disposed;

        public bool AutoCommit { get; set; }

        internal UnitOfWork(IServiceProvider serviceProvider, bool autoCommit)
        {
	        _serviceProvider = serviceProvider;
	        _uowProvider = serviceProvider.GetService(typeof(IUoWProvider)) as IUoWProvider 
	                       ?? throw new NullReferenceException($"Failed to get service {typeof(IUoWProvider).FullName}");
	        _eventHub = serviceProvider.GetService(typeof(IEventHub)) as IEventHub;
            AutoCommit = autoCommit;
        }

        public void ResolveRepository<TR, TE>(out TR? repository) where TR : class, IRepository<TE> where TE : Entity, IAggregateRoot
        {
	        repository = _serviceProvider.GetService(typeof(TR)) as TR;

            if(repository != null)
				_uowProvider.JoinUoW(repository);
        }

        public virtual async Task<int> SaveAllEntitiesAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_uowProvider.ChangedEntities.Count <= 0)
                return 0;

            if (_eventHub != null)
				await FlushDomainEventsAsync(_eventHub, _uowProvider);

            return await _uowProvider.SaveChangesAsync(cancellationToken);
        }

        private static async Task FlushDomainEventsAsync(IEventHub eventHub, IUoWProvider uowContext)
        {
	        var entities = uowContext.ChangedEntities.Where(e => e.DomainEvents?.Any() ?? false).ToList();

	        var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

	        var tasks = domainEvents.Select(e => (Task)eventHub.PublishAsync((dynamic)e));

	        await Task.WhenAll(tasks);

	        entities.ForEach(e => e.ClearDomainEvents());
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
                    if (SaveAllEntitiesAsync().Result < 0)
                        throw new ApplicationException($"Failed to save changed entities. +(UnitOfWork provider type: '{_uowProvider.GetType().FullName}') ");
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
