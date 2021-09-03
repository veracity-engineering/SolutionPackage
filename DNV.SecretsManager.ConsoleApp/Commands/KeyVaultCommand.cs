using DNV.SecretsManager.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	public class KeyVaultCommand : Command
	{
		public string Url { get; set; }

		public override async Task Execute()
		{
			if (Type == CommandType.Download)
			{
				Console.WriteLine($"Downloading secrets from Azure KeyVault '{Url}' to file '{TargetFilename}'...");
				Console.WriteLine("Please wait.");
				var result = await DownloadKeyVaultSecrets(Url, TargetFilename);
				Console.WriteLine($"Download complete. Downloaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
			}
			if (Type == CommandType.Upload)
			{
				Console.WriteLine($"Uploading secrets from file '{TargetFilename}' to Azure KeyVault '{Url}'...");
				Console.WriteLine("Please wait.");
				var result = await UploadKeyVaultSecrets(TargetFilename, Url);
				Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} secrets in {result.ElapsedTime.TotalSeconds:f2}s.");
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
	}
}
