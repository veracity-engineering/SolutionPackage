using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	internal abstract class Command
	{
		public CommandType Type { get; set; }

		public string TargetFilename { get; set; }

		public abstract Task<CommandResult> Execute();
	}
}
