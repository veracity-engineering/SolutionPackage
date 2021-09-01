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
					LastSources = new Dictionary<string, string>[]
					{
						new Dictionary<string, string>(),
						new Dictionary<string, string>()
					}
				};
			}
			return storage;
		}

		public void Save()
		{
			if (!Directory.Exists(StoragePath))
				Directory.CreateDirectory(StoragePath);
			File.WriteAllText(StorageFilename, JsonConvert.SerializeObject(this));
		}

		public void SetLast(int typeIndex, Dictionary<string, string> source)
		{
			LastSourceTypeIndex = typeIndex;
			LastSources[LastSourceTypeIndex] = source;
		}
	}
}
