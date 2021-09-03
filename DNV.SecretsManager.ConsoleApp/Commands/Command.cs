using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	public abstract class Command
	{
		public CommandType Type { get; set; }

		public string TargetFilename { get; set; }

		public abstract Task Execute();
	}
}
