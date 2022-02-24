using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DNV.Application.Abstractions;
using DNVGL.Domain.EventHub.MediatR.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNV.DDD.Test
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMrEventHub(typeof(Startup));
		}
	}
}
