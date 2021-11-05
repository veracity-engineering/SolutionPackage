using DNV.SecretsManager.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	internal class KeyVaultCommand : IConsoleCommand
	{
		public IEnumerable<ConsoleOption> Options { get; } = new[]
		{
			new ConsoleOption { Name = "download", Abbreviation = 'd', IsFlag = true },
			new ConsoleOption { Name = "upload", Abbreviation = 'u', IsFlag = true },
			new ConsoleOption { Name = "url", Abbreviation = 's' },
			new ConsoleOption { Name = "filename", Abbreviation = 'f' }
		};

		private static readonly Dictionary<char, CommandType> _commandTypes = new Dictionary<char, CommandType>
		{
			{ 'd', CommandType.Download },
			{ 'u', CommandType.Upload }
		};

		public string Description { get; } = "Download or upload secrets to/from Azure Keyvault";

		public CommandType Type { get; set; }

		public string Url { get; set; }

		public string Filename { get; set; }

		public IConsoleCommand Build(Dictionary<string, object> options)
		{
			if (options.ContainsKey("download") && options.ContainsKey("upload"))
				throw new ArgumentException("Both instructions for download and upload were provided.");
			if (options.ContainsKey("download"))
				Type = CommandType.Download;
			if (options.ContainsKey("upload"))
				Type = CommandType.Upload;

			if (options.ContainsKey("url"))
				Url = options["url"].ToString();

			if (options.ContainsKey("filename"))
				Filename = options["filename"].ToString();

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

			// Url
			if (string.IsNullOrEmpty(Url))
				Console.WriteLine("Please enter the URL for the Azure KeyVault:");
			while (string.IsNullOrEmpty(Url))
			{
				var keyVaultBaseUrl = Console.ReadLine();
				if (ValidationUtility.IsUriValid(keyVaultBaseUrl))
				{
					Url = keyVaultBaseUrl;
				}
				else
				{
					Console.WriteLine("Invalid url format. Please enter a fully qualified URL for the Azure KeyVault (for e.g: https://dnv.com):");
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
				Console.WriteLine($"Downloading secrets from Azure KeyVault '{Url}' to file '{Filename}'...");
				var result = await DownloadKeyVaultSecrets(Url, Filename);
				Console.WriteLine($"Download complete. Downloaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
				return;
			}
			if (Type == CommandType.Upload)
			{
				Console.WriteLine($"Uploading secrets from file '{Filename}' to Azure KeyVault '{Url}'...");
				var result = await UploadKeyVaultSecrets(Url, Filename);
				Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
				return;
			}
			DisplayHelp();
		}

		private static async Task<CommandResult> DownloadKeyVaultSecrets(string keyVaultBaseUrl, string targetFilename)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new KeyVaultSecretsService();
			var secrets = await secretsService.GetSecretsAsDictionary(keyVaultBaseUrl);
			var result = secretsService.ToJson(secrets);
			await File.WriteAllTextAsync(targetFilename, result, System.Text.Encoding.UTF8);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private static async Task<CommandResult> UploadKeyVaultSecrets(string sourceFilename, string keyVaultBaseUrl)
		{
			var stopwatch = Stopwatch.StartNew();
			var content = await File.ReadAllTextAsync(sourceFilename);
			var secretsService = new KeyVaultSecretsService();
			var secrets = secretsService.FromJson(content);
			await secretsService.SetSecretsFromJson(keyVaultBaseUrl, content);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private void DisplayHelp()
		{
			Console.WriteLine($"usage: secretsmanager keyvault\t--download | -d | --upload | -u <url> -f <filename>");
		}
	}
}
