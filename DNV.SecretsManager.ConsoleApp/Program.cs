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
		private const char HelpKey = '?';
		private const char KeyVaultKey = 'k';
		private const char VariableGroupKey = 'v';
		private const char DownloadKey = 'd';
		private const char UploadKey = 'u';

		private static readonly Dictionary<char, Func<Command>> _commands = new Dictionary<char, Func<Command>>
		{
			{ KeyVaultKey, () => new KeyVaultCommand() },
			{ VariableGroupKey, () => new VariableGroupCommand() }
		};

		private static readonly Dictionary<char, CommandType> _commandTypes = new Dictionary<char, CommandType>
		{
			{ DownloadKey, CommandType.Download },
			{ UploadKey, CommandType.Upload }
		};

		private static readonly List<ConsoleOption> _optionDefinitions = new List<ConsoleOption>
		{
			new ConsoleOption { Name = "download", Abbreviation = DownloadKey, IsFlag = true },
			new ConsoleOption { Name = "upload", Abbreviation = UploadKey, IsFlag = true },

			new ConsoleOption { Name = "help", Abbreviation = 'h', IsFlag = true },
			new ConsoleOption { Name = "help", Abbreviation = HelpKey, IsFlag = true},

			new ConsoleOption { Name = "keyvault", Abbreviation = KeyVaultKey, IsFlag = true },
			new ConsoleOption { Name = "keyvault-url", Abbreviation = 's' },

			new ConsoleOption { Name = "variablegroup", Abbreviation = VariableGroupKey, IsFlag = true },
			new ConsoleOption { Name = "variablegroup-base-url", Abbreviation = 's' },
			new ConsoleOption { Name = "variablegroup-organization", Abbreviation  ='o' },
			new ConsoleOption { Name = "variablegroup-pat", Abbreviation = 'p' },

			new ConsoleOption { Name = "filename", Abbreviation = 'f' }
		};

		private static Command FromOptions(Dictionary<string, object> options)
		{
			Command command = null;
			
			if (options.ContainsKey("keyvault") && options.ContainsKey("variablegroup"))
				throw new ArgumentException("Both instructions for keyvault and variable group were provided.");
			if (options.ContainsKey("keyvault"))
				command = new KeyVaultCommand();
			if (options.ContainsKey("variablegroup"))
				command = new VariableGroupCommand();

			if (options.ContainsKey("download") && options.ContainsKey("upload"))
				throw new ArgumentException("Both instructions for download and upload were provided.");
			if (options.ContainsKey("download"))
				command.Type = CommandType.Download;
			if (options.ContainsKey("upload"))
				command.Type = CommandType.Upload;

			if (command is KeyVaultCommand kvc)
			{
				if (options.ContainsKey("keyvault-url"))
					kvc.Url = options["keyvault-url"].ToString();
			}
			if (command is VariableGroupCommand vgc)
			{
				if (options.ContainsKey("variablegroup-base-url"))
					vgc.BaseUrl = options["variablegroup-base-url"].ToString();
				if (options.ContainsKey("variablegroup-organization"))
					vgc.Organization = options["variablegroup-organization"].ToString();
				if (options.ContainsKey("variablegroup-pat"))
					vgc.PersonalAccessToken = options["variablegroup-pat"].ToString();
			}

			if (options.ContainsKey("filename"))
				command.TargetFilename = options["filename"].ToString();

			return command;
		}

		static async Task Main(string[] args)
		{
			var options = CollectOptions(args);
			Command command = null;
			if (options!= null && options.Any())
			{
				if (options.ContainsKey("help"))
				{
					DisplayHelp();
					return;
				}	
				command = FromOptions(options);
			}

			if (command == null)
				Console.WriteLine($"Select source (Azure KeyVault: [{KeyVaultKey}], Azure DevOps Variable Group: [{VariableGroupKey}])");
			while (command == null)
			{
				var sourceChoice = $"{Console.ReadKey().Key}".ToLowerInvariant()[0];
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
				var commandTypeChoice = $"{Console.ReadKey().Key}".ToLowerInvariant()[0];
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
					Console.WriteLine("Please enter Organization or Project name:");
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
						if (File.Exists(sourceFilename))
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

		private static Dictionary<string, object> CollectOptions(string[] args)
		{
			var options = new Dictionary<string, object>();
			var argumentIndex = 0;
			//for (var i = 0; i < args.Length; i++)
			while (argumentIndex < args.Length)
			{
				var arg = args[argumentIndex];

				// Long option
				ConsoleOption optionDefinition = null;
				if (arg.StartsWith("--"))
				{
					optionDefinition = _optionDefinitions.FirstOrDefault(o => o.Name.Equals(arg.Substring(2, arg.Length - 2)));
				}
				// Short option
				else if (arg.StartsWith('-'))
				{
					optionDefinition = _optionDefinitions.FirstOrDefault(o => o.Abbreviation.Equals(arg[1]));
				}

				if (optionDefinition == null)
					throw new ArgumentException($"Unrecognized argument '{arg}'.");

				if (optionDefinition.IsFlag)
				{
					options.Add(optionDefinition.Name, true);
				}
				else
				{
					options.Add(optionDefinition.Name, args[++argumentIndex]);
				}
				argumentIndex++;
			}

			return options;
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
	}
}
