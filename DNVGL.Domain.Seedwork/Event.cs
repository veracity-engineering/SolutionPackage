namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Event
    {
        protected Event(Entity eventSource)
        {
            EventSource = eventSource;
        }

        public abstract EventType EventType { get; }

        public Entity EventSource { get; }
    }
}
