using DNV.SecretsManager.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	internal class VariableGroupCommand : IConsoleCommand
	{
		public string Name { get; } = "variablegroup";

		public string Description { get; } = "Download or upload secrets from/to Azure Keyvault";

		public IEnumerable<ConsoleOption> Options { get; } = new[]
		{
			new ConsoleOption { Name = "help", Abbreviation = 'h', IsFlag = true, IsOptional = true },
			new ConsoleOption { Name = "download", Abbreviation = 'd', IsFlag = true },
			new ConsoleOption { Name = "upload", Abbreviation = 'u', IsFlag = true },
			new ConsoleOption { Name = "clear", Abbreviation = 'c', IsFlag = true },
			new ConsoleOption { Name = "base-url", Abbreviation = 's' },
			new ConsoleOption { Name = "organization", Abbreviation  ='o' },
			new ConsoleOption { Name = "pat", Abbreviation = 'p' },
			new ConsoleOption { Name = "group-id", Abbreviation = 'g' },
			new ConsoleOption { Name = "filename", Abbreviation = 'f' }
		};

		public CommandType Type { get; set; }

		public string BaseUrl { get; set; }

		public string Organization { get; set; }

		public string PersonalAccessToken { get; set; }

		public string GroupId { get; set; }

		public string Filename { get; set; }

		private readonly string _applicationName;

		public VariableGroupCommand(string applicationName)
		{
			_applicationName = applicationName;
		}

		public IConsoleCommand Build(Dictionary<string, object> options)
		{
			if (options.ContainsKey("help"))
				return this;

			// Assign from options
			if (options.ContainsKey("download") && options.ContainsKey("upload"))
				throw new ArgumentException("Both instructions for download and upload were provided.");
			if (options.ContainsKey("download"))
				Type = CommandType.Download;
			if (options.ContainsKey("upload"))
				Type = CommandType.Upload;
			if (options.ContainsKey("clear"))
				Type = CommandType.Clear;

			if (options.ContainsKey("base-url"))
				BaseUrl = options["base-url"].ToString();

			if (options.ContainsKey("organization"))
				Organization = options["organization"].ToString();

			if (options.ContainsKey("pat"))
				PersonalAccessToken = options["pat"].ToString();

			if (options.ContainsKey("group-id"))
				GroupId = options["group-id"].ToString();

			// Collect args not provided in initial call.
			// Type
			var downloadOption = Options.First(o => o.Name.Equals("download"));
			var uploadOption = Options.First(o => o.Name.Equals("upload"));
			var clearOption = Options.First(o => o.Name.Equals("clear"));
			Type = ConsoleCommand.GetCommandTypeOrInvalid(Type, downloadOption, uploadOption, clearOption);

			// Base url
			BaseUrl = ConsoleCommand.GetStringOrInvalid(BaseUrl,
				"Please enter the base URL for Azure DevOps instance:",
				ValidationUtility.IsUriValid,
				i => "Invalid url format. Please enter a fully qualified URL for the Azure DevOps instance (for e.g: https://dnv.com):"
			);

			// Organization
			Organization = ConsoleCommand.GetStringOrInvalid(Organization,
				"Please enter Organization or Project name:",
				i => !string.IsNullOrWhiteSpace(i),
				i => $"Invalid value '{i}'.  Please enter an Organization or Project name that is not empty and not whitespace:"
			);

			// PAT
			PersonalAccessToken = ConsoleCommand.GetStringOrInvalid(PersonalAccessToken,
				"Please enter Personal Access Token (PAT):",
				i => !string.IsNullOrWhiteSpace(i),
				i => $"Invalid value '{i}'.  Please enter an Personal Access Token that is not empty and not whitespace:"
			);

			// Variable group id
			GroupId = ConsoleCommand.GetStringOrInvalid(GroupId,
				"Please enter the id of the Variable Group in Azure DevOps:",
				i => int.TryParse(i, out int _),
				i => $"Invalid value '{i}'.  Please enter a numeric value for the Variable Group id:"
			);

			// Filename
			Filename = ConsoleCommand.GetFilenameOrInvalid(Type, Filename);

			return this;
		}

		public async Task Execute()
		{
			if (Type == CommandType.Download)
			{
				Console.WriteLine($"Downloading variables from Variable Group '{GroupId}' to file '{Filename}'...");
				var result = await DownloadVariableGroup(GroupId, Filename);
				Console.WriteLine($"Download complete. Downloaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
				return;
			}
			if (Type == CommandType.Upload)
			{
				Console.WriteLine($"Uploading variables from file '{Filename}' to Variable Group'{GroupId}'...");
				var result = await UploadVariableGroup(Filename, GroupId);
				Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
				return;
			}
			if (Type == CommandType.Clear)
			{
				Console.WriteLine($"Clearing all variables in Variable Group '{GroupId}'...");
				var result = await ClearKeyVaultSecrets(GroupId);
				Console.WriteLine($"Clear complete.  Cleared {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
				return;
			}
			DisplayHelp();
		}

		private async Task<CommandResult> DownloadVariableGroup(string variableGroupId, string targetFilename)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new VariableGroupSecretsService(ToConfiguration(this));
			var secrets = Task.Run(async () => await secretsService.GetSecretsAsDictionary(variableGroupId)).GetAwaiter().GetResult();
			var result = secretsService.ToJson(secrets);
			await File.WriteAllTextAsync(targetFilename, result, System.Text.Encoding.UTF8);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private async Task<CommandResult> UploadVariableGroup(string sourceFilename, string variableGroupId)
		{
			var stopwatch = Stopwatch.StartNew();
			var content = await File.ReadAllTextAsync(sourceFilename);
			var secretsService = new VariableGroupSecretsService(ToConfiguration(this));
			var secrets = secretsService.FromJson(content);
			await secretsService.SetSecretsFromJson(variableGroupId, content);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private async Task<CommandResult> ClearKeyVaultSecrets(string variableGroupId)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new VariableGroupSecretsService(ToConfiguration(this));
			var deletedCount = await secretsService.ClearSecrets(variableGroupId);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = deletedCount,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private static VariableGroupClientConfiguration ToConfiguration(VariableGroupCommand command) =>
			new VariableGroupClientConfiguration
			{
				BaseUrl = command.BaseUrl,
				Organization = command.Organization,
				PersonalAccessToken = command.PersonalAccessToken
			};

		private void DisplayHelp()
		{
			Console.WriteLine(ConsoleCommand.BuildCommandUseage(this, _applicationName));
		}
	}
}