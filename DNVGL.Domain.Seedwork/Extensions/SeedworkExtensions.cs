using System.Linq;
using System.Threading.Tasks;

namespace DNVGL.Domain.Seedwork.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SeedworkExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IEventHub eventHub, IUnitOfWorkProvider uowContext)
        {
            var entities = uowContext.ChangedEntities.Where(e => e.DomainEvents?.Any() ?? false).ToList();

            var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

            entities.ForEach(e => e.ClearDomainEvents());

            var tasks = domainEvents.Select(e => eventHub.PublishAsync(e));

            await Task.WhenAll(tasks);
        }
    }
}
