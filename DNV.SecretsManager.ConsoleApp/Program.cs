using DNV.SecretsManager.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp
{
	class Program
	{
		private static VariableGroupClientConfiguration _variableGroupClientConfiguration;

		static async Task Main(string[] args)
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
					var result = await DownloadKeyVaultSecrets(keyVaultBaseUrl, targetFilename);
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
					var result = await UploadKeyVaultSecrets(sourceFilename, keyVaultBaseUrl);
					Console.WriteLine($"Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
				}
			}
			else if (sourceChoice.Equals("v"))
			{
				Console.WriteLine("Please enter the base URL for Azure DevOps instance:");
				var baseUrl = Console.ReadLine();

				Console.WriteLine("Please enter Organization or Poject name:");
				var organization = Console.ReadLine();

				Console.WriteLine("Please enter Personal Access Token (PAT):");
				var pat = Console.ReadLine();

				_variableGroupClientConfiguration = new VariableGroupClientConfiguration
				{
					BaseUrl = baseUrl,
					Organization = organization,
					PersonalAccessToken = pat
				};

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
					var result = await DownloadVariableGroup(variableGroupId, targetFilename);
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
					var result = await UploadVariableGroup(sourceFilename, variableGroupId);
					Console.WriteLine($"Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
				}
			}
		}

		private static async Task<CommandResult> DownloadKeyVaultSecrets(string keyVaultBaseUrl, string targetFilename)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new KeyVaultSecretsService();
			try
			{
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
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private static async Task<CommandResult> DownloadVariableGroup(string variableGroupId, string targetFilename)
		{
			var stopwatch = Stopwatch.StartNew();
			var secretsService = new VariableGroupSecretsService(_variableGroupClientConfiguration);
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

		private static async Task<CommandResult> UploadKeyVaultSecrets(string sourceFilename, string keyVaultBaseUrl)
		{
			var stopwatch = Stopwatch.StartNew();
			var content = await File.ReadAllTextAsync(sourceFilename);
			var secretsService = new KeyVaultSecretsService();
			var secrets = secretsService.FromJson(content);
			try
			{
				await secretsService.SetSecretsFromJson(keyVaultBaseUrl, content);
				stopwatch.Stop();
				return new CommandResult
				{
					Count = secrets.Count,
					ElapsedTime = stopwatch.Elapsed
				};
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private static async Task<CommandResult> UploadVariableGroup(string sourceFilename, string variableGroupId)
		{
			var stopwatch = Stopwatch.StartNew();
			var content = await File.ReadAllTextAsync(sourceFilename);
			var secretsService = new VariableGroupSecretsService(_variableGroupClientConfiguration);
			var secrets = secretsService.FromJson(content);
			await secretsService.SetSecretsFromJson(variableGroupId, content);
			stopwatch.Stop();
			return new CommandResult
			{
				Count = secrets.Count,
				ElapsedTime = stopwatch.Elapsed
			};
		}
	}
}
