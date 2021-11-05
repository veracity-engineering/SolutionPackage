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
		public IEnumerable<ConsoleOption> Options { get; } = new[]
		{
			new ConsoleOption { Name = "download", Abbreviation = 'd', IsFlag = true },
			new ConsoleOption { Name = "upload", Abbreviation = 'u', IsFlag = true },
			new ConsoleOption { Name = "base-url", Abbreviation = 's' },
			new ConsoleOption { Name = "organization", Abbreviation  ='o' },
			new ConsoleOption { Name = "pat", Abbreviation = 'p' },
			new ConsoleOption { Name = "group-id", Abbreviation = 'g' },
			new ConsoleOption { Name = "filename", Abbreviation = 'f' }
		};

		private static readonly Dictionary<char, CommandType> _commandTypes = new Dictionary<char, CommandType>
		{
			{ 'd', CommandType.Download },
			{ 'u', CommandType.Upload }
		};

		public string Description { get; } = "Download or upload secrets to/from Azure Keyvault";

		public CommandType Type { get; set; }

		public string BaseUrl { get; set; }

		public string Organization { get; set; }

		public string PersonalAccessToken { get; set; }

		public string VariableGroupId { get; set; }

		public string Filename { get; set; }

		public IConsoleCommand Build(Dictionary<string, object> options)
		{
			if (options.ContainsKey("download") && options.ContainsKey("upload"))
				throw new ArgumentException("Both instructions for download and upload were provided.");
			if (options.ContainsKey("download"))
				Type = CommandType.Download;
			if (options.ContainsKey("upload"))
				Type = CommandType.Upload;

			if (options.ContainsKey("base-url"))
				BaseUrl = options["base-url"].ToString();

			if (options.ContainsKey("organization"))
				Organization = options["organization"].ToString();

			if (options.ContainsKey("pat"))
				PersonalAccessToken = options["pat"].ToString();

			if (options.ContainsKey("group-id"))
				VariableGroupId = options["group-id"].ToString();

			// Collect args not provided in initial call.
			// Type
			var downloadOption = Options.First(o => o.Name.Equals("download"));
			var uploadOption = Options.First(o => o.Name.Equals("upload"));
			if (Type == CommandType.None)
				Console.WriteLine($"What would you like to do? (Download: [{downloadOption.Abbreviation}], Upload: [{uploadOption.Abbreviation}])");
			while (Type == CommandType.None)
			{
				var commandTypeChoice = $"{Console.ReadKey().Key}".ToLowerInvariant()[0];
				Console.WriteLine();
				if (_commandTypes.ContainsKey(commandTypeChoice))
				{
					Type = _commandTypes[commandTypeChoice];
				}
				else
				{
					Console.WriteLine($"Invalid option '{commandTypeChoice}'. Please enter a valid option (Download: [{downloadOption.Abbreviation}], Upload: [{uploadOption.Abbreviation}])");
				}
			}

			// Base url
			if (string.IsNullOrEmpty(BaseUrl))
				Console.WriteLine("Please enter the base URL for Azure DevOps instance:");
			while (string.IsNullOrEmpty(BaseUrl))
			{
				var baseUrl = Console.ReadLine();
				if (ValidationUtility.IsUriValid(baseUrl))
				{
					BaseUrl = baseUrl;
				}
				else
				{
					Console.WriteLine("Invalid url format. Please enter a fully qualified URL for the Azure DevOps instance (for e.g: https://dnv.com):");
				}
			}

			// Organization
			if (string.IsNullOrEmpty(Organization))
				Console.WriteLine("Please enter Organization or Project name:");
			while (string.IsNullOrEmpty(Organization))
			{
				var organization = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(organization))
				{
					Organization = organization;
				}
				else
				{
					Console.WriteLine($"Invalid value '{organization}'.  Please enter an Organization or Project name that is not empty and not whitespace:");
				}
			}

			// PAT
			if (string.IsNullOrEmpty(PersonalAccessToken))
				Console.WriteLine("Please enter Personal Access Token (PAT):");
			while (string.IsNullOrEmpty(PersonalAccessToken))
			{
				var pat = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(pat))
				{
					PersonalAccessToken = pat;
				}
				else
				{
					Console.WriteLine($"Invalid value '{pat}'.  Please enter an Personal Access Token that is not empty and not whitespace:");
				}
			}

			// Variable group id
			if (string.IsNullOrEmpty(VariableGroupId))
				Console.WriteLine("Please enter the id of the Variable Group in Azure DevOps:");
			while (string.IsNullOrEmpty(VariableGroupId))
			{
				var variableGroupId = Console.ReadLine();
				if (int.TryParse(variableGroupId, out int _))
				{
					VariableGroupId = variableGroupId;
				}
				else
				{
					Console.WriteLine($"Invalid value '{variableGroupId}'.  Please enter a numeric value for the Variable Group id:");
				}
			}

			// Filename
			if (Type == CommandType.Download)
			{
				if (string.IsNullOrEmpty(Filename))
					Console.WriteLine("Specify the target filename you would like to download to:");
				while (string.IsNullOrEmpty(Filename))
				{
					var targetFilename = Console.ReadLine();
					if (ValidationUtility.IsFilenameValid(targetFilename))
					{
						Filename = targetFilename;
					}
					else
					{
						Console.WriteLine($"Invaild filename '{targetFilename}'. Please specify a valid filename to download to:");
					}
				}
			}
			else if (Type == CommandType.Upload)
			{
				if (string.IsNullOrEmpty(Filename))
					Console.WriteLine("Specify the source file you would like to upload:");
				while (string.IsNullOrEmpty(Filename))
				{
					var sourceFilename = Console.ReadLine();
					if (ValidationUtility.IsFilenameValid(sourceFilename))
					{
						if (File.Exists(sourceFilename))
						{
							Filename = sourceFilename;
						}
						else
						{
							Console.WriteLine($"Could not find source file '{Filename}'.  Please specify an existing file to upload:");
						}
					}
					else
					{
						Console.WriteLine($"Invalid filename '{Filename}'. Please specify a valid filename to upload:");
					}
				}
			}

			return this;
		}

		public async Task Execute()
		{
			if (Type == CommandType.Download)
			{
				Console.WriteLine($"Downloading variables from Variable Group '{VariableGroupId}' to file '{Filename}'...");
				var result = await DownloadVariableGroup(VariableGroupId, Filename);
				Console.WriteLine($"Download complete. Downloaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
				return;
			}
			if (Type == CommandType.Upload)
			{
				Console.WriteLine($"Uploading variables from file '{Filename}' to Variable Group'{VariableGroupId}'...");
				var result = await UploadVariableGroup(Filename, VariableGroupId);
				Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
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

		private static VariableGroupClientConfiguration ToConfiguration(VariableGroupCommand command) =>
			new VariableGroupClientConfiguration
			{
				BaseUrl = command.BaseUrl,
				Organization = command.Organization,
				PersonalAccessToken = command.PersonalAccessToken
			};

		private void DisplayHelp()
		{
			Console.WriteLine($"usage: secretsmanager variablegroup\t--download | -d | --upload | -u <url> -o | --organization <organization> -p | --pat <personal access token> -g | --group-id <variable group id> -f | --filename <filename>");
		}
	}
}