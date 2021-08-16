using DNV.SecretsManager.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp
{
	class Program
	{
		private const string AzureSubscriptionId = "d288cb4f-5356-481f-a571-11005977e590";

		private static VariableGroupClientConfiguration _variableGroupClientConfiguration = new VariableGroupClientConfiguration
		{
			BaseUrl = "https://dnvgl-one.visualstudio.com",
			Organization = "Veracity",
			ApiVersion = "6.1-preview.2",
			PersonalAccessToken = "4gepm6cenmvsc3cox2hzymkddvof4dkx5xpwgi6x34tfcylyl6pa"
		};

		static void Main(string[] args)
		{
			Console.WriteLine("Select source (Azure KeyVault: [k], Azure DevOps Variable Group: [v])");
			var sourceChoice = Console.ReadLine().ToLowerInvariant();

			if (sourceChoice.Equals("k"))
			{
				Console.WriteLine("Please enter the URL for the Azure KeyVault:");
				var keyVaultBaseUrl = Console.ReadLine();

				Console.WriteLine("What would you like to do? (Upload: [u], Download: [d])");
				var taskChoice = Console.ReadLine().ToLowerInvariant();

				if (taskChoice == "d")
				{
					Console.WriteLine("Specify the target filename you would like to download to:");
					var targetFilename = Console.ReadLine();

					Console.WriteLine($"Downloading secrets from '{keyVaultBaseUrl}'...");
					Console.WriteLine("Please wait.");
					var result = DownloadKeyVaultSecrets(keyVaultBaseUrl, targetFilename);
					Console.WriteLine($"Downloaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
					Console.WriteLine($"Generated file: '{targetFilename}'.");
				}
				else if (taskChoice == "u")
				{
					Console.WriteLine("Specify the source file you would like to upload:");
					var sourceFilename = Console.ReadLine();
					if (!File.Exists(sourceFilename))
					{
						Console.WriteLine($"Error: could not find source file '{sourceFilename}'.");
						return;
					}

					Console.WriteLine($"Uploading secrets form '{sourceFilename}' to '{keyVaultBaseUrl}'...");
					Console.WriteLine("Please wait.");
					var result = UploadKeyVaultSecrets(sourceFilename, keyVaultBaseUrl);
					Console.WriteLine($"Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
				}
			}
			else if (sourceChoice.Equals("v"))
			{
				Console.WriteLine("Please enter the id of the Variable Group in Azure DevOps:");
				var variableGroupId = Console.ReadLine();

				Console.WriteLine("What would you like to do? (Upload: [u], Download: [d])");
				var taskChoice = Console.ReadLine().ToLowerInvariant();

				if (taskChoice == "d")
				{
					Console.WriteLine("Specify the target filename you would like to download to:");
					var targetFilename = Console.ReadLine();

					Console.WriteLine($"Downloading variables from variable group '{variableGroupId}'...");
					Console.WriteLine("Please wait.");
					var result = DownloadVariableGroup(variableGroupId, targetFilename);
					Console.WriteLine($"Downloaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
					Console.WriteLine($"Generated file: '{targetFilename}'.");
				}
				else if (taskChoice == "u")
				{
					Console.WriteLine("Specify the source file you would like to upload:");
					var sourceFilename = Console.ReadLine();
					if (!File.Exists(sourceFilename))
					{
						Console.WriteLine($"Error: could not find source file '{sourceFilename}'.");
						return;
					}

					Console.WriteLine($"Uploading variables form '{sourceFilename}' to variable group'{variableGroupId}'...");
					Console.WriteLine("Please wait.");
					var result = UploadVariableGroup(sourceFilename, variableGroupId);
					Console.WriteLine($"Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
				}
			}
		}

		private static CommandResult DownloadKeyVaultSecrets(string keyVaultBaseUrl, string targetFilename)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new KeyVaultSecretsService(AzureSubscriptionId);
			var secrets = Task.Run(async () => await secretsService.GetSecretsAsDictionary(keyVaultBaseUrl)).GetAwaiter().GetResult();
			var result = secretsService.ToJson(secrets);
			File.WriteAllText(targetFilename, result, System.Text.Encoding.UTF8);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private static CommandResult DownloadVariableGroup(string variableGroupId, string targetFilename)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new VariableGroupSecretsService(_variableGroupClientConfiguration);
			var secrets = Task.Run(async () => await secretsService.GetSecretsAsDictionary(variableGroupId)).GetAwaiter().GetResult();
			var result = secretsService.ToJson(secrets);
			File.WriteAllText(targetFilename, result, System.Text.Encoding.UTF8);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private static CommandResult UploadKeyVaultSecrets(string sourceFilename, string keyVaultBaseUrl)
		{
			var stopwatch = Stopwatch.StartNew();
			var content = File.ReadAllText(sourceFilename);
			var secretsService = new KeyVaultSecretsService(AzureSubscriptionId);
			var secrets = secretsService.FromJson(content);
			Task.Run(async () => await secretsService.SetSecretsFromJson(keyVaultBaseUrl, content)).GetAwaiter();
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}

		private static CommandResult UploadVariableGroup(string sourceFilename, string variableGroupId)
		{
			var stopwatch = Stopwatch.StartNew();
			var content = File.ReadAllText(sourceFilename);
			var secretsService = new VariableGroupSecretsService(_variableGroupClientConfiguration);
			var secrets = secretsService.FromJson(content);
			Task.Run(async () => await secretsService.SetSecretsFromJson(variableGroupId, content)).GetAwaiter();
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}
	}
}
