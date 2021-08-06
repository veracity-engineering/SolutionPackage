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
		public string ApiVersion { get; set; }
		public string PersonalAccessToken { get; set; }
	}

	internal class VariableGroupClient
	{
		/*
		private const string BaseUrl = "https://dnvgl-one.visualstudio.com";
		private const string Organization = "Veracity";
		private const string ApiVersion = "6.1-preview.2";
		private const string PersonalAccessToken = "4gepm6cenmvsc3cox2hzymkddvof4dkx5xpwgi6x34tfcylyl6pa";
		*/

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
				using (HttpResponseMessage response = client.GetAsync(url).Result)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					response.EnsureSuccessStatusCode();
					return JsonConvert.DeserializeObject<VariableGroup>(responseBody);
				}
			}
		}

		public async Task<IEnumerable<VariableGroup>> GetVariableGroups()
		{
			var url = $"{_configuration.BaseUrl}/{_configuration.Organization}/_apis/distributedtask/variablegroups?api-version={_configuration.ApiVersion}";

			using (var client = GetAuthenticatedClient())
			{
				using (HttpResponseMessage response = client.GetAsync(url).Result)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					response.EnsureSuccessStatusCode();
					return JsonConvert.DeserializeObject<VariableGroupCollection>(responseBody).Value;
				}
			}
		}

		public async Task SetVariableGroup(string groupId, VariableGroup group)
		{
			var url = $"{_configuration.BaseUrl}/{_configuration.Organization}/_apis/distributedtask/variablegroups/{groupId}?api-version={_configuration.ApiVersion}";

			using (var client = GetAuthenticatedClient())
			{
				var content = new StringContent(JsonConvert.SerializeObject(group), Encoding.UTF8, "application/json");
				using (HttpResponseMessage response = client.PutAsync(url, content).Result)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					response.EnsureSuccessStatusCode();
				}
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
