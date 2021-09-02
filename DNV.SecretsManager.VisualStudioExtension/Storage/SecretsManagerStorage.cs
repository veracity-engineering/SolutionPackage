using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNV.SecretsManager.VisualStudioExtension.Storage
{
	internal class SecretsManagerStorage
	{
		[JsonProperty("lastSourceType")]
		public int LastSourceTypeIndex { get; set; }

		[JsonProperty("last")]
		public Dictionary<string, string>[] LastSources { get; set; }

		[JsonProperty("sources")]
		public SourceCache[] Sources { get; set; }

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
					LastSourceTypeIndex = -1,
					LastSources = new Dictionary<string, string>[]
					{
						new Dictionary<string, string>(),
						new Dictionary<string, string>()
					},
					Sources = new SourceCache[]
					{
						new SourceCache(),
						new SourceCache()
					}
				};
			}
			return storage;
		}

		public void Save()
		{
			if (!Directory.Exists(StoragePath))
				Directory.CreateDirectory(StoragePath);
			File.WriteAllText(StorageFilename, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public void SetLast(int typeIndex, Dictionary<string, string> source)
		{
			LastSourceTypeIndex = typeIndex;
			LastSources[LastSourceTypeIndex] = source;
		}

		public void SetSources(int typeIndex, SourceCache sourceCache)
		{
			Sources[typeIndex] = sourceCache;
		}
	}
}

internal class SourceCache
{
	[JsonProperty("parent")]
	public Dictionary<string, string> Parent { get; set; }

	[JsonProperty("sources")]
	public Dictionary<string, string> Sources { get; set; }
}