using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Entity
    {
        private int _hashCode;

        public string Id { get; set; }

        private List<Event> _domainEvents;

        public IReadOnlyCollection<Event>? DomainEvents => _domainEvents?.AsReadOnly();

        // comment out because of EF required?
        /*protected Entity(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            Id = id;
        }*/

        public void AddDomainEvent(Event @event)
        {
            _domainEvents = _domainEvents ?? new List<Event>();
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(Event @event)
        {
            _domainEvents?.Remove(@event);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return Id == string.Empty;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();

            if (_hashCode == 0)
            {
                Interlocked.CompareExchange(ref _hashCode,
                    CalculateHashCode(), 0);
            }

            return _hashCode;
        }

        protected virtual int CalculateHashCode()
        {
            return Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            if (entity.IsTransient() || IsTransient())
                return false;

            return Id == entity.Id;
        }
    }
}
