using System.Threading;
using System.Threading.Tasks;

namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventHandler<in T> where T: Event
    {
        Task HandleAsync(T @event, CancellationToken cancellationToken = default);
    }
}