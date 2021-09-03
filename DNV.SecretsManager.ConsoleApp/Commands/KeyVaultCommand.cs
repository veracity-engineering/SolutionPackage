using DNV.SecretsManager.Services;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	internal class KeyVaultCommand : Command
	{
		public string Url { get; set; }

		public override Task<CommandResult> Execute()
		{
			if (Type == CommandType.Download)
				return DownloadKeyVaultSecrets(Url, TargetFilename);
			if (Type == CommandType.Upload)
				return UploadKeyVaultSecrets(TargetFilename, Url);
			return Task.FromResult<CommandResult>(null);
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
	}
}
