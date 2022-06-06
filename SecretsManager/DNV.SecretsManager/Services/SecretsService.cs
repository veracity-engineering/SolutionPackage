using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNV.SecretsManager.Services
{
	public abstract class SecretsService
	{
		public abstract Task<IEnumerable<KeyValuePair<string, string>>> GetSources();

		public abstract Task<string> GetSecretsAsJson(string source);

		public abstract Task SetSecretsFromJson(string source, string json);

		public abstract Task<Dictionary<string, string>> GetSecretsAsDictionary(string source);

		public abstract Task SetSecretsFromDictionary(string vaultBaseUrl, Dictionary<string, string> secrets);

		public abstract Task<int> ClearSecrets(string source);

		public string ToJson(Dictionary<string, string> secrets) =>
			JsonConvert.SerializeObject(JsonFlattener.Unflatten(secrets), Formatting.Indented);

		public Dictionary<string, string> FromJson(string json) =>
			JsonFlattener.Flatten(JsonConvert.DeserializeObject<JObject>(json));
	}
}
