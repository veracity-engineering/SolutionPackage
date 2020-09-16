using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DNVGL.OAuth.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();

			//Host.CreateDefaultBuilder(args)
			//	.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
		}
	}
}
