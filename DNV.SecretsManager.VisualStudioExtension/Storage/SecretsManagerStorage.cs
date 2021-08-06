using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DNV.SecretsManager.VisualStudioExtension.Storage
{
	internal class SecretsManagerStorage
	{
		[JsonProperty("sourceTypes")]
		public SecretsSources[] SourceTypes { get; set; }

		[JsonProperty("lastSourceType")]
		public int LastSourceTypeIndex { get; set; }

		[JsonIgnore]
		public static string StoragePath => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.dnv.secretsmanager";

		[JsonIgnore]
		private static string StorageFilename => $"{StoragePath}/storage.json";

		public static SecretsManagerStorage LoadOrCreate(string[] sourceTypes)
		{
			SecretsManagerStorage storage;
			if (File.Exists(StorageFilename))
			{
				storage = JsonConvert.DeserializeObject<SecretsManagerStorage>(File.ReadAllText(StorageFilename));
			}
			else
			{
				storage = new SecretsManagerStorage
				{
					SourceTypes = sourceTypes.Select(t => new SecretsSources()).ToArray(),
					LastSourceTypeIndex = -1
				};
			}
			return storage;
		}

		public void AppendSource(int sourceTypeIndex, string source)
		{
			var sourceType = SourceTypes[sourceTypeIndex];
			if (sourceType.Sources == null)
			{
				sourceType.Sources = new[] { source };
			}
			else if (!sourceType.Sources.Any(s => s.Equals(source, StringComparison.InvariantCultureIgnoreCase)))
			{
				var sources = sourceType.Sources.ToList();
				sources.Add(source);
				sourceType.Sources = sources;
			}
			LastSourceTypeIndex = sourceTypeIndex;
			sourceType.Last = source;
		}

		public void Save()
		{
			if (!Directory.Exists(StoragePath))
				Directory.CreateDirectory(StoragePath);
			File.WriteAllText(StorageFilename, JsonConvert.SerializeObject(this));
		}
	}

	internal class SecretsSources
	{
		[JsonProperty("sources")]
		public IEnumerable<string> Sources;

		[JsonProperty("last")]
		public string Last { get; set; }
	}
}
