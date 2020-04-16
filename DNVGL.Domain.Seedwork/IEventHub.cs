using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventHub
    {
        Task PublishAsync(Event @event, CancellationToken cancellationToken = default);
    }
}