using DNVGL.Domain.EventHub.MediatR.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DNV.Domain.Tests
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMrEventHub(typeof(Startup));
		}
	}
}
