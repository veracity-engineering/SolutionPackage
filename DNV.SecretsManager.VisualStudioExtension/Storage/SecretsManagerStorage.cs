using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DNV.SecretsManager.VisualStudioExtension.Storage
{
	internal class SecretsManagerStorage
	{
		[JsonProperty("lastSourceType")]
		public int LastSourceTypeIndex { get; set; }

		[JsonProperty("sources")]
		public List<SourceCache> Sources { get; set; }

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
					Sources = new List<SourceCache>()
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

		public void Delete() => File.Delete(StorageFilename);

		public void SetLast(int lastSourceTypeIndex, KeyValuePair<string, string>? parent, Dictionary<string, string> source)
		{
			LastSourceTypeIndex = lastSourceTypeIndex;
			if (Sources.Any(s => IsParentMatch(s, parent)))
				Sources.First(s => IsParentMatch(s, parent)).Last = source;
		}

		public Dictionary<string, string> GetLast(KeyValuePair<string, string>? parent)
		{
			return Sources.FirstOrDefault(s => IsParentMatch(s, parent)).Last;
		}

		public bool HasSourceCache(KeyValuePair<string, string>? parent)
		{
			return Sources.Any(s => IsParentMatch(s, parent) && s.Sources != null);
		}

		public Dictionary<string, string> GetCachedSource(KeyValuePair<string, string>? parent)
		{
			return Sources.FirstOrDefault(s => IsParentMatch(s, parent)).Sources;
		}

		public void AppendSourceCache(KeyValuePair<string, string>? parent, List<KeyValuePair<string, string>> sources)
		{
			AppendSourceCache(parent, sources.ToDictionary(s => s.Key, s => s.Value));
		}

		public void AppendSourceCache(KeyValuePair<string, string>? parent, Dictionary<string, string> sources)
		{
			if (!parent.HasValue)
			{
				var existingSource = Sources.FirstOrDefault(s => IsParentMatch(s, parent));
				if (existingSource == null)
				{
					var sourceCache = new SourceCache
					{
						Parent = null,
						Sources = sources
					};
					Sources.Add(sourceCache);
				}
			}
			else
			{
				var existingSource = Sources.FirstOrDefault(s => IsParentMatch(s, parent));
				if (existingSource == null)
				{
					var sourceCache = new SourceCache
					{
						Parent = new Dictionary<string, string> { { parent.Value.Key, parent.Value.Value } },
						Sources = sources
					};
					Sources.Add(sourceCache);
				}
				else
				{
					if (existingSource.Sources == null)
					{
						Sources.First(s => IsParentMatch(s, parent))
							.Sources = sources;
					}
				}
			}
		}

		private bool IsParentMatch(SourceCache sourceCache, KeyValuePair<string, string>? parent)
		{
			return parent.HasValue
				? sourceCache.Parent != null && sourceCache.Parent.Values.Contains(parent.Value.Value)
				: sourceCache.Parent == null;
		}
	}
}

internal class SourceCache
{
	[JsonProperty("parent")]
	public Dictionary<string, string> Parent { get; set; }

	[JsonProperty("sources")]
	public Dictionary<string, string> Sources { get; set; }

	[JsonProperty("last")]
	public Dictionary<string, string> Last { get; set; }
}