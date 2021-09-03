using DNV.SecretsManager.ConsoleApp.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp
{
	class Program
	{
		private const string KeyVaultKey = "k";
		private const string VariableGroupKey = "v";
		private const string DownloadKey = "d";
		private const string UploadKey = "u";

		private static readonly Dictionary<string, Func<Command>> _commands = new Dictionary<string, Func<Command>>
		{
			{ KeyVaultKey, () => new KeyVaultCommand() },
			{ VariableGroupKey, () => new VariableGroupCommand() }
		};

		private static readonly Dictionary<string, CommandType> _commandTypes = new Dictionary<string, CommandType>
		{
			{ DownloadKey, CommandType.Download },
			{ UploadKey, CommandType.Upload }
		};

		static async Task Main(string[] args)
		{
			Command command = null;
			var commandTypes = new Dictionary<string, CommandType>
			{
				{ DownloadKey, CommandType.Download },
				{ UploadKey, CommandType.Upload }
			};

			if (args.Any())
				command = FromArgs(args, _commands, _commandTypes);

			if (command == null)
			{
				Console.WriteLine($"Select source (Azure KeyVault: [{KeyVaultKey}], Azure DevOps Variable Group: [{VariableGroupKey}])");
				var sourceChoice = Console.ReadLine().ToLowerInvariant();
				command = _commands[sourceChoice]();
			}

			if (command.Type == CommandType.None)
			{
				Console.WriteLine($"What would you like to do? (Download: [{DownloadKey}], Upload: [{UploadKey}])");
				var commandTypeChoice = Console.ReadLine().ToLowerInvariant();
				command.Type = commandTypes[commandTypeChoice];
			}

			if (command is KeyVaultCommand keyVaultCommand)
			{
				if (string.IsNullOrEmpty(keyVaultCommand.Url))
				{
					Console.WriteLine("Please enter the URL for the Azure KeyVault:");
					var keyVaultBaseUrl = Console.ReadLine();
					keyVaultCommand.Url = keyVaultBaseUrl;
				}

				if (keyVaultCommand.Type == CommandType.Download)
				{
					if (string.IsNullOrEmpty(keyVaultCommand.TargetFilename))
					{
						Console.WriteLine("Specify the target filename you would like to download to:");
						var targetFilename = Console.ReadLine();
						keyVaultCommand.TargetFilename = targetFilename;
					}
				}
				else if (keyVaultCommand.Type == CommandType.Upload)
				{
					if (string.IsNullOrEmpty(keyVaultCommand.TargetFilename))
					{
						Console.WriteLine("Specify the source file you would like to upload:");
						var sourceFilename = Console.ReadLine();
						keyVaultCommand.TargetFilename = sourceFilename;
					}
					if (!File.Exists(keyVaultCommand.TargetFilename))
					{
						Console.WriteLine($"Error: could not find source file '{keyVaultCommand.TargetFilename}'.");
						return;
					}
				}
			}
			else if (command is VariableGroupCommand variableGroupCommand)
			{
				if (string.IsNullOrEmpty(variableGroupCommand.BaseUrl))
				{
					Console.WriteLine("Please enter the base URL for Azure DevOps instance:");
					var baseUrl = Console.ReadLine();
					variableGroupCommand.BaseUrl = baseUrl;
				}

				if (string.IsNullOrEmpty(variableGroupCommand.Organization))
				{
					Console.WriteLine("Please enter Organization or Poject name:");
					var organization = Console.ReadLine();
					variableGroupCommand.Organization = organization;
				}

				if (string.IsNullOrEmpty(variableGroupCommand.PersonalAccessToken))
				{
					Console.WriteLine("Please enter Personal Access Token (PAT):");
					var pat = Console.ReadLine();
					variableGroupCommand.PersonalAccessToken = pat;
				}

				if (string.IsNullOrEmpty(variableGroupCommand.VariableGroupId))
				{
					Console.WriteLine("Please enter the id of the Variable Group in Azure DevOps:");
					var variableGroupId = Console.ReadLine();
					variableGroupCommand.VariableGroupId = variableGroupId;
				}

				if (variableGroupCommand.Type == CommandType.Download)
				{
					if (string.IsNullOrEmpty(variableGroupCommand.TargetFilename))
					{
						Console.WriteLine("Specify the target filename you would like to download to:");
						var targetFilename = Console.ReadLine();
						variableGroupCommand.TargetFilename = targetFilename;
					}
				}
				else if (variableGroupCommand.Type == CommandType.Upload)
				{
					if (string.IsNullOrEmpty(variableGroupCommand.TargetFilename))
					{
						Console.WriteLine("Specify the source file you would like to upload:");
						var sourceFilename = Console.ReadLine();
						variableGroupCommand.TargetFilename = sourceFilename;
					}
					if (!File.Exists(variableGroupCommand.TargetFilename))
					{
						Console.WriteLine($"Error: could not find source file '{variableGroupCommand.TargetFilename}'.");
						return;
					}
				}
			}
			try
			{
				await command.Execute();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}

		private static Command FromArgs(string[] args, Dictionary<string, Func<Command>> commands, Dictionary<string, CommandType> commandTypes)
		{
			Command command = null;
			command = commands[args[0].ToLowerInvariant()]();

			if (args.Length > 1)
				command.Type = commandTypes[args[1].ToLowerInvariant()];

			if (args.Length > 2)
			{
				if (command is KeyVaultCommand keyVaultCommand)
				{
					keyVaultCommand.Url = args[2];
					if (args.Length > 3)
						keyVaultCommand.TargetFilename = args[3];
				}

				if (command is VariableGroupCommand variableGroupCommand)
				{
					variableGroupCommand.BaseUrl = args[2];
					if (args.Length > 3)
						variableGroupCommand.Organization = args[3];
					if (args.Length > 4)
						variableGroupCommand.PersonalAccessToken = args[4];
					if (args.Length > 5)
						variableGroupCommand.TargetFilename = args[5];
				}
			}
			return command;
		}
	}
}
