using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	internal interface IConsoleCommand
	{
		public IEnumerable<ConsoleOption> Options { get; }

		public string Description { get; }

		public IConsoleCommand Build(Dictionary<string, object> options);

		public Task Execute();
	}
}
