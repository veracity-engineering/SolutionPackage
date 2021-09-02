using Newtonsoft.Json;
using System;
using System.IO;

namespace DNV.SecretsManager.VisualStudioExtension.Storage
{
	public class SecretsManagerConfiguration
	{
		[JsonProperty("variableGroups")]
		public VariableGroupsConfiguration VariableGroups { get; set; }

		[JsonProperty("allowUpload")]
		public bool IsAllowUpload { get; set; }

		[JsonIgnore]
		public static string Path => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.dnv.secretsmanager";

		[JsonIgnore]
		public static string Filename => $"{Path}/config.json";

		public static SecretsManagerConfiguration Load()
		{
			if (File.Exists(Filename))
				return JsonConvert.DeserializeObject<SecretsManagerConfiguration>(File.ReadAllText(Filename));
			return null;
		}

		public void Save()
		{
			if (!Directory.Exists(Path))
				Directory.CreateDirectory(Path);
			File.WriteAllText(Filename, JsonConvert.SerializeObject(this));
		}
	}

	public class VariableGroupsConfiguration
	{
		[JsonProperty("url")]
		public string BaseUrl { get; set; }

		[JsonProperty("organization")]
		public string Organization { get; set; }

		[JsonProperty("pat")]
		public string PersonalAccessToken { get; set; }
	}
}
