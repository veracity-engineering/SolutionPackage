using System.Threading;
using System.Threading.Tasks;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventHub
    {
        Task PublishAsync(Event @event, CancellationToken cancellationToken = default);
    }
}