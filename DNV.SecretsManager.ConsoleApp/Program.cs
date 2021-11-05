﻿using DNV.SecretsManager.ConsoleApp.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp
{
	class Program
	{
		private static readonly Dictionary<string, IConsoleCommand> _commands = new Dictionary<string, IConsoleCommand>
		{
			{ "keyvault", new KeyVaultCommand(GettApplicationName()) },
			{ "variablegroup", new VariableGroupCommand(GettApplicationName()) }
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
			Console.WriteLine($"usage: {GettApplicationName()}\t<command> [<args>]");
			Console.WriteLine();
			Console.WriteLine("Commands:");
			foreach (var command in _commands)
			{
				Console.WriteLine($"\t{command.Key}\t{command.Value.Description}");
			}
		}

		private static string GettApplicationName()
		{
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			var filename = Path.GetFileName(codeBase);
			return filename.Substring(0, filename.Length - ".exe".Length);
		}
	}
}
