using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.SecretsManager.Services
{
	public class VariableGroupSecretsService : SecretsService
	{
		private readonly VariableGroupClient _client;

		public VariableGroupSecretsService(VariableGroupClientConfiguration configuration)
		{
			_client = new VariableGroupClient(configuration);
		}

		public override async Task<IEnumerable<KeyValuePair<string, string>>> GetSources()
		{
			var sources = await _client.GetVariableGroups();
			return sources.Select(g => new KeyValuePair<string, string>(g.Name, $"{g.Id}"));
		}

		public override async Task<string> GetSecretsAsJson(string variableGroupId) =>
			ToJson(await GetSecretsAsDictionary(variableGroupId));

		public override Task SetSecretsFromJson(string variableGroupId, string json) =>
			SetSecretsFromDictionary(variableGroupId, FromJson(json));

		public override async Task<Dictionary<string, string>> GetSecretsAsDictionary(string variableGroupId)
		{
			var result = await _client.GetVariableGroup(variableGroupId);
			return result.Variables.ToDictionary(v => v.Key, v => v.Value.Value);
		}

		public override async Task SetSecretsFromDictionary(string variableGroupId, Dictionary<string, string> secrets)
		{
			var group = await _client.GetVariableGroup(variableGroupId);
			group.Variables = secrets.ToDictionary(s => s.Key, s => new VariableValue
			{
				Value = s.Value,
				IsReadOnly = group.Variables.ContainsKey(s.Key) && group.Variables[s.Key].IsReadOnly,
				IsSecret = group.Variables.ContainsKey(s.Key) && group.Variables[s.Key].IsSecret
			});
			await _client.SetVariableGroup(variableGroupId, group);
		}
	}
}
