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
		private const string HelpKey = "?";
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

			if (args.Any())
			{
				if (args[0].Equals(HelpKey, StringComparison.InvariantCultureIgnoreCase))
				{
					DisplayHelp();
					return;
				}
				command = FromArgs(args, _commands, _commandTypes);
			}

			if (command == null)
				Console.WriteLine($"Select source (Azure KeyVault: [{KeyVaultKey}], Azure DevOps Variable Group: [{VariableGroupKey}])");
			while (command == null)
			{
				var sourceChoice = $"{Console.ReadKey().Key}".ToLowerInvariant();
				Console.WriteLine();
				if (_commands.ContainsKey(sourceChoice))
				{
					command = _commands[sourceChoice]();
				}
				else
				{
					Console.WriteLine($"Invalid option '{sourceChoice}'. Please enter a valid option (Azure KeyVault: [{KeyVaultKey}], Azure DevOps Variable Group: [{VariableGroupKey}])");
				}
			}

			if (command.Type == CommandType.None)
				Console.WriteLine($"What would you like to do? (Download: [{DownloadKey}], Upload: [{UploadKey}])");
			while (command.Type == CommandType.None)
			{
				var commandTypeChoice = $"{Console.ReadKey().Key}".ToLowerInvariant();
				Console.WriteLine();
				if (_commandTypes.ContainsKey(commandTypeChoice))
				{
					command.Type = _commandTypes[commandTypeChoice];
				}
				else
				{
					Console.WriteLine($"Invalid option '{commandTypeChoice}'. Please enter a valid option (Download: [{DownloadKey}], Upload: [{UploadKey}])");
				}
			}

			if (command is KeyVaultCommand keyVaultCommand)
			{
				if (string.IsNullOrEmpty(keyVaultCommand.Url))
					Console.WriteLine("Please enter the URL for the Azure KeyVault:");
				while (string.IsNullOrEmpty(keyVaultCommand.Url))
				{
					var keyVaultBaseUrl = Console.ReadLine();
					if (ValidationUtility.IsUriValid(keyVaultBaseUrl))
					{
						keyVaultCommand.Url = keyVaultBaseUrl;
					}
					else
					{
						Console.WriteLine("Invalid url format. Please enter a fully qualified URL for the Azure KeyVault (for e.g: https://dnv.com):");
					}
				}
			}
			else if (command is VariableGroupCommand variableGroupCommand)
			{
				if (string.IsNullOrEmpty(variableGroupCommand.BaseUrl))
					Console.WriteLine("Please enter the base URL for Azure DevOps instance:");
				while (string.IsNullOrEmpty(variableGroupCommand.BaseUrl))
				{
					var baseUrl = Console.ReadLine();
					if (ValidationUtility.IsUriValid(baseUrl))
					{
						variableGroupCommand.BaseUrl = baseUrl;
					}
					else
					{
						Console.WriteLine("Invalid url format. Please enter a fully qualified URL for the Azure DevOps instance (for e.g: https://dnv.com):");
					}
				}

				if (string.IsNullOrEmpty(variableGroupCommand.Organization))
					Console.WriteLine("Please enter Organization or Poject name:");
				while (string.IsNullOrEmpty(variableGroupCommand.Organization))
				{
					var organization = Console.ReadLine();
					if (!string.IsNullOrWhiteSpace(organization))
					{
						variableGroupCommand.Organization = organization;
					}
					else
					{
						Console.WriteLine($"Invalid value '{organization}'.  Please enter an Organization or Project name that is not empty and not whitespace:");
					}
				}

				if (string.IsNullOrEmpty(variableGroupCommand.PersonalAccessToken))
					Console.WriteLine("Please enter Personal Access Token (PAT):");
				while (string.IsNullOrEmpty(variableGroupCommand.PersonalAccessToken))
				{
					var pat = Console.ReadLine();
					if (string.IsNullOrWhiteSpace(pat))
					{
						variableGroupCommand.PersonalAccessToken = pat;
					}
					else
					{
						Console.WriteLine($"Invalid value '{pat}'.  Please enter an Personal Access Token that is not empty and not whitespace:");
					}
				}

				if (string.IsNullOrEmpty(variableGroupCommand.VariableGroupId))
					Console.WriteLine("Please enter the id of the Variable Group in Azure DevOps:");
				while (string.IsNullOrEmpty(variableGroupCommand.VariableGroupId))
				{
					var variableGroupId = Console.ReadLine();
					if (int.TryParse(variableGroupId, out int _))
					{
						variableGroupCommand.VariableGroupId = variableGroupId;
					}
					else
					{
						Console.WriteLine($"Invalid value '{variableGroupId}'.  Please enter a numeric value for the Variable Group id:");
					}
				}
			}
			if (command.Type == CommandType.Download)
			{
				if (string.IsNullOrEmpty(command.TargetFilename))
					Console.WriteLine("Specify the target filename you would like to download to:");
				while (string.IsNullOrEmpty(command.TargetFilename))
				{
					var targetFilename = Console.ReadLine();
					if (ValidationUtility.IsFilenameValid(targetFilename))
					{
						command.TargetFilename = targetFilename;
					}
					else
					{
						Console.WriteLine($"Invaild filename '{targetFilename}'. Please specify a valid filename to download to:");
					}
				}
			}
			else if (command.Type == CommandType.Upload)
			{
				if (string.IsNullOrEmpty(command.TargetFilename))
					Console.WriteLine("Specify the source file you would like to upload:");
				while (string.IsNullOrEmpty(command.TargetFilename))
				{
					var sourceFilename = Console.ReadLine();
					if (ValidationUtility.IsFilenameValid(sourceFilename))
					{
						if (File.Exists(command.TargetFilename))
						{
							command.TargetFilename = sourceFilename;
						}
						else
						{
							Console.WriteLine($"Could not find source file '{command.TargetFilename}'.  Please specify an existing file to upload:");
						}
					}
					else
					{
						Console.WriteLine($"Invalid filename '{command.TargetFilename}'. Please specify a valid filename to upload:");
					}
				}
			}

			if (command.Type == CommandType.Download)
			{
				if (command is KeyVaultCommand kvc)
					Console.WriteLine($"Downloading secrets from Azure KeyVault '{kvc.Url}' to file '{command.TargetFilename}'...");
				if (command is VariableGroupCommand vgc)
					Console.WriteLine($"Downloading variables from Variable Group '{vgc.VariableGroupId}' to file '{command.TargetFilename}'...");
			}
			else if (command.Type == CommandType.Upload)
			{
				if (command is KeyVaultCommand kvc)
					Console.WriteLine($"Uploading secrets from file '{command.TargetFilename}' to Azure KeyVault '{kvc.Url}'...");
				if (command is VariableGroupCommand vgc)
					Console.WriteLine($"Uploading variables from file '{command.TargetFilename}' to Variable Group'{vgc.VariableGroupId}'...");
			}
			Console.WriteLine("Please wait.");

			try
			{
				var result = await command.Execute();
				if (command.Type == CommandType.Download)
				{
					if (command is KeyVaultCommand resultKvc)
						Console.WriteLine($"Download complete. Downloaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
					if (command is VariableGroupCommand resultVgc)
						Console.WriteLine($"Download complete. Downloaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
				}
				if (command.Type == CommandType.Upload)
				{
					if (command is KeyVaultCommand resultKvc)
						Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
					if (command is VariableGroupCommand resultVgc)
						Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
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
			Console.WriteLine("Downloads or upload secrets from an Azure KeyVault or an Azure DevOps Variable Group.");
			Console.WriteLine();
			Console.WriteLine("For Azure KayVault:");
			Console.WriteLine($"\t{KeyVaultKey} {DownloadKey}|{UploadKey} [url] [filename]");
			Console.WriteLine();
			Console.WriteLine("For Azure DevOps Variable Group:");
			Console.WriteLine($"\t{VariableGroupKey} {DownloadKey}|{UploadKey} [url] [organization] [personal access token] [variable group id] [filename]");
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
