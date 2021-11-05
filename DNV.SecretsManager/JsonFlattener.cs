using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNV.SecretsManager
{
	internal class JsonFlattener
	{
		private enum JsonType
		{
			OBJECT,
			ARRAY
		}

		private const string PathDelimeter = "--";

		public static Dictionary<string, string> Flatten(JObject jsonObject)
		{
			var jTokens = jsonObject.Descendants().Where(p => p.Count() == 0);
			var results = jTokens.Aggregate(new Dictionary<string, string>(), (properties, jToken) =>
			{
				properties.Add(jToken.Path.Replace(".", PathDelimeter).Replace("[", PathDelimeter).Replace("]", ""), jToken.ToString());
				return properties;
			});
			return results;
		}

		public static JObject Unflatten(IDictionary<string, string> keyValues)
		{
			JContainer result = null;
			var setting = new JsonMergeSettings
			{
				MergeArrayHandling = MergeArrayHandling.Merge
			};
			foreach (var pathValue in keyValues)
			{
				if (result == null)
				{
					result = UnflattenSingle(pathValue);
				}
				else
				{
					result.Merge(UnflattenSingle(pathValue), setting);
				}
			}
			return result as JObject;
		}

		private static JContainer UnflattenSingle(KeyValuePair<string, string> keyValue)
		{
			var path = keyValue.Key;
			var value = keyValue.Value;
			var pathSegments = SplitPath(path);

			JContainer lastItem = null;

			//build from leaf to root
			foreach (var pathSegment in pathSegments.Reverse())
			{
				switch (GetJsonType(pathSegment))
				{
					case JsonType.OBJECT:
						lastItem = UnflattenObject(lastItem, pathSegment, value);
						break;
					case JsonType.ARRAY:
						lastItem = UnflattenArray(lastItem, pathSegment, value);
						break;
				}
			}
			return lastItem;
		}

		private static JObject UnflattenObject(JContainer lastItem, string pathSegment, string value)
		{
			var obj = new JObject();
			if (lastItem == null)
			{
				obj.Add(pathSegment, value);
			}
			else
			{
				obj.Add(pathSegment, lastItem);
			}
			return obj;
		}

		private static JArray UnflattenArray(JContainer lastItem, string pathSegment, string value)
		{
			var array = new JArray();
			int index = GetArrayIndex(pathSegment);
			array = FillEmpty(array, index);
			if (lastItem == null)
			{
				array[index] = value;
			}
			else
			{
				array[index] = lastItem;
			}
			return array;
		}

		public static string[] SplitPath(string path) =>
			path.Split(new string[] { PathDelimeter }, StringSplitOptions.None).ToArray();

		private static JArray FillEmpty(JArray array, int index)
		{
			for (int i = 0; i <= index; i++)
			{
				array.Add(null);
			}
			return array;
		}

		private static JsonType GetJsonType(string pathSegment)
		{
			return int.TryParse(pathSegment, out var x)
				? JsonType.ARRAY
				: JsonType.OBJECT;
		}

		private static int GetArrayIndex(string pathSegment)
		{
			if (int.TryParse(pathSegment, out var result))
				return result;
			throw new Exception($"Unable to parse array index: {pathSegment}");
		}
	}
}
