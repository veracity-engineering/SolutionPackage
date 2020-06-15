using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DNVGL.SolutionPackage.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
		}
	}
}
