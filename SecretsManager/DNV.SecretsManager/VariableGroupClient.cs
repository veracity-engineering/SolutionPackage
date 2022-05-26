using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DNV.SecretsManager
{
	public class VariableGroupClientConfiguration
	{
		public string BaseUrl { get; set; }
		public string Organization { get; set; }
		public string ApiVersion { get; set; } = "6.1-preview.2";
		public string PersonalAccessToken { get; set; }
	}

	internal class VariableGroupClient
	{
		private readonly VariableGroupClientConfiguration _configuration;

		public VariableGroupClient(VariableGroupClientConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<VariableGroup> GetVariableGroup(string groupId)
		{
			var url = $"{_configuration.BaseUrl}/{_configuration.Organization}/_apis/distributedtask/variablegroups/{groupId}?api-version={_configuration.ApiVersion}";

			using (var client = GetAuthenticatedClient())
			{
				var response = await client.GetAsync(url);
				var responseBody = await response.Content.ReadAsStringAsync();
				response.EnsureSuccessStatusCode();
				return JsonConvert.DeserializeObject<VariableGroup>(responseBody);
			}
		}

		public async Task<IEnumerable<VariableGroup>> GetVariableGroups()
		{
			var url = $"{_configuration.BaseUrl}/{_configuration.Organization}/_apis/distributedtask/variablegroups?api-version={_configuration.ApiVersion}";

			using (var client = GetAuthenticatedClient())
			{
				var response = await client.GetAsync(url);
				var responseBody = await response.Content.ReadAsStringAsync();
				response.EnsureSuccessStatusCode();
				return JsonConvert.DeserializeObject<VariableGroupCollection>(responseBody).Value;
			}
		}

		public async Task SetVariableGroup(string groupId, VariableGroup group)
		{
			var url = $"{_configuration.BaseUrl}/{_configuration.Organization}/_apis/distributedtask/variablegroups/{groupId}?api-version={_configuration.ApiVersion}";

			using (var client = GetAuthenticatedClient())
			{
				var content = new StringContent(JsonConvert.SerializeObject(group), Encoding.UTF8, "application/json");
				var response = await client.PutAsync(url, content);
				response.EnsureSuccessStatusCode();
			}
		}

		private HttpClient GetAuthenticatedClient()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{null}:{_configuration.PersonalAccessToken}")));
			return client;
		}
	}

	internal class VariableGroupCollection
	{
		public int Count { get; set; }

		public IEnumerable<VariableGroup> Value { get; set; }
	}

	internal class VariableGroup
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Type { get; set; }

		public string Description { get; set; }

		public Dictionary<string, VariableValue> Variables { get; set; }

		public VariableGroupProjectReference[] VariableGroupProjectReferences { get; set; }

		public VariableGroupProviderData ProviderData { get; set; }
	}

	internal class VariableValue
	{
		public bool IsReadOnly { get; set; }

		public bool IsSecret { get; set; }

		public string Value { get; set; }
	}

	internal class VariableGroupProjectReference
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public ProjectReference ProjectReference { get; set; }
	}

	internal class VariableGroupProviderData
	{
		public bool? IsReadOnly { get; set; }

		public bool? IsSecret { get; set; }

		public string Value { get; set; }
	}

	internal class ProjectReference
	{
		public string Id { get; set; }

		public string Name { get; set; }
	}
}
