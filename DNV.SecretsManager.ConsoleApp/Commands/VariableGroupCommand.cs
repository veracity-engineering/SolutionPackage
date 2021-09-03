using DNV.SecretsManager.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	public class VariableGroupCommand : Command
	{
		public string BaseUrl { get; set; }

		public string Organization { get; set; }

		public string PersonalAccessToken { get; set; }

		public string VariableGroupId { get; set; }

		public override async Task Execute()
		{
			if (Type == CommandType.Download)
			{
				Console.WriteLine($"Downloading variables from Variable Group '{VariableGroupId}' to file '{TargetFilename}'...");
				Console.WriteLine("Please wait.");
				var result = await DownloadVariableGroup(VariableGroupId, TargetFilename);
				Console.WriteLine($"Download complete. Downloaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
			}
			if (Type == CommandType.Upload)
			{
				Console.WriteLine($"Uploading variables from file '{TargetFilename}' to Variable Group'{VariableGroupId}'...");
				Console.WriteLine("Please wait.");
				var result = await UploadVariableGroup(TargetFilename, VariableGroupId);
				Console.WriteLine($"Upload complete. Uploaded {result.Count:n0} variables in {result.ElapsedTime.TotalSeconds:f2}s.");
			}
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
	}
}