using DNV.SecretsManager.ConsoleApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp
{
	class Program
	{
		private static readonly Dictionary<string, IConsoleCommand> _commands = new Dictionary<string, IConsoleCommand>
		{
			{ "keyvault", new KeyVaultCommand() },
			{ "variablegroup", new VariableGroupCommand() }
		};

		static async Task Main(string[] args)
		{
			try
			{

				if (args != null && args.Any())
				{
					var commandName = args[0];

					if (_commands.ContainsKey(commandName))
					{
						var command = _commands[commandName];
						var options = ConsoleCommand.CollectOptions(command.Options, args.Skip(1).ToArray());
						command = command.Build(options);
						await command.Execute();
					}
					else
					{
						Console.WriteLine($"Unrecognized command '{commandName}'.");
					}
				}
				else
				{
					DisplayHelp();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}

		private static void DisplayHelp()
		{
			Console.WriteLine($"usage: secretsmanager\t<command> [<args>]");
			Console.WriteLine();
			Console.WriteLine("Commands:");
			foreach (var command in _commands)
			{
				Console.WriteLine($"\t{command.Key}\t{command.Value.Description}");
			}
		}
	}
}
