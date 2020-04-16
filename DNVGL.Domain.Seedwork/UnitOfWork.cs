﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork.Extensions;

namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitOfWork: IUnitOfWork
    {
        private readonly IUnitOfWorkProvider _uowContext;

        private readonly IEventHub _eventHub;

        private int _disposed;

        public bool AutoCommit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public UnitOfWork(IUnitOfWorkProvider uowContext, IEventHub eventHub)
        {
            _uowContext = uowContext ?? throw new ArgumentNullException(nameof(uowContext));
            _eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
        }

        public virtual async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_uowContext.ChangedEntities.Count <= 0)
                return true;

            await _eventHub.DispatchDomainEventsAsync(_uowContext);

            var result = await _uowContext.SaveChangesAsync(cancellationToken);

            return result >= 0;
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
