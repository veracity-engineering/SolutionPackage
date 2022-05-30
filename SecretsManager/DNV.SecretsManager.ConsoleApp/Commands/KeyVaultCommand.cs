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
		public string Name { get; } = "keyvault";

		public string Description { get; } = "Download or upload secrets from/to Azure Keyvault";

		public IEnumerable<ConsoleOption> Options { get; } = new[]
		{
			new ConsoleOption { Name = "help", Abbreviation = 'h', IsFlag = true, IsOptional = true },
			new ConsoleOption { Name = "download", Abbreviation = 'd', IsFlag = true },
			new ConsoleOption { Name = "upload", Abbreviation = 'u', IsFlag = true },
			new ConsoleOption { Name = "clear", Abbreviation = 'c', IsFlag = true },
			new ConsoleOption { Name = "url", Abbreviation = 's' },
			new ConsoleOption { Name = "filename", Abbreviation = 'f' }
		};

		public CommandType Type { get; set; }

		public string Url { get; set; }

		public string Filename { get; set; }

		private readonly string _applicationName;

		public KeyVaultCommand(string applicationName)
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

			if (options.ContainsKey("url"))
				Url = options["url"].ToString();

			if (options.ContainsKey("filename"))
				Filename = options["filename"].ToString();

			// Collect args not provided in initial call.
			// Type
			var downloadOption = Options.First(o => o.Name.Equals("download"));
			var uploadOption = Options.First(o => o.Name.Equals("upload"));
			var clearOption = Options.First(o => o.Name.Equals("clear"));
			Type = ConsoleCommand.GetCommandTypeOrInvalid(Type, downloadOption, uploadOption, clearOption);

			// Url
			Url = ConsoleCommand.GetStringOrInvalid(Url,
				"Please enter the URL for the Azure KeyVault:",
				ValidationUtility.IsUriValid,
				i => "Invalid url format. Please enter a fully qualified URL for the Azure KeyVault (for e.g: https://dnv.com):"
			);

			// Filename
			Filename = ConsoleCommand.GetFilenameOrInvalid(Type, Filename);

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
				var result = await UploadKeyVaultSecrets(Filename, Url);
				Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
				return;
			}
			if (Type == CommandType.Clear)
			{
				Console.WriteLine($"Clearing all secrets in KeyVault '{Url}'...");
				var result = await ClearKeyVaultSecrets(Url);
				Console.WriteLine($"Clear complete.  Cleared {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
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

		private static async Task<CommandResult> ClearKeyVaultSecrets(string keyVaultBaseUrl)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new KeyVaultSecretsService();
			var deletedCount = await secretsService.ClearSecrets(keyVaultBaseUrl);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = deletedCount,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private void DisplayHelp()
		{
			Console.WriteLine(ConsoleCommand.BuildCommandUseage(this, _applicationName));
		}
	}
}
