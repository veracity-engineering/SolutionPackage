namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EventType : Enumeration<string>
    {
        private EventType(string @namespace, string id, string name) : base($"{@namespace}/{id}", name)
        {
            Namespace = @namespace;
        }

        public string Namespace { get; }
    }
}