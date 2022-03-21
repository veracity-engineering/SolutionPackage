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
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T: Event;
    }
}