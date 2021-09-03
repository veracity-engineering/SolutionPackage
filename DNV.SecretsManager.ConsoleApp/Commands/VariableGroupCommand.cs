using DNV.SecretsManager.Services;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	internal class VariableGroupCommand : Command
	{
		public string BaseUrl { get; set; }

		public string Organization { get; set; }

		public string PersonalAccessToken { get; set; }

		public string VariableGroupId { get; set; }

		public override Task<CommandResult> Execute()
		{
			if (Type == CommandType.Download)
				return DownloadVariableGroup(VariableGroupId, TargetFilename);
			if (Type == CommandType.Upload)
				return UploadVariableGroup(TargetFilename, VariableGroupId);
			return Task.FromResult<CommandResult>(null);
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