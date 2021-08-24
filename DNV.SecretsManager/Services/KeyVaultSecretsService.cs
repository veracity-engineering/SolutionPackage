using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Services.AppAuthentication;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Rest;
using System;

namespace DNV.SecretsManager.Services
{
	public class KeyVaultSecretsService : SecretsService
	{
		private readonly string _azureSubscriptionId;

		public KeyVaultSecretsService(string azureSubscriptionKey)
		{
			_azureSubscriptionId = azureSubscriptionKey;
		}

		public override async Task<IEnumerable<KeyValuePair<string, string>>> GetSources()
		{
			try
			{
				var azureTokenProvider = new AzureServiceTokenProvider();
				var credentials = new TokenCredentials(await azureTokenProvider.GetAccessTokenAsync("https://management.azure.com/").ConfigureAwait(false));
				var client = new KeyVaultManagementClient(credentials)
				{
					SubscriptionId = _azureSubscriptionId
				};
				var result = await client.Vaults.ListBySubscriptionAsync();
				var keyvaults = result.ToDictionary(v => v.Name, v => v.Properties.VaultUri);
				while (!string.IsNullOrEmpty(result.NextPageLink))
				{
					result = await client.Vaults.ListBySubscriptionNextAsync(result.NextPageLink);
					foreach (var keyvault in result)
					{
						keyvaults.Add(keyvault.Name, keyvault.Properties.VaultUri);
					}
				}
				return keyvaults.OrderBy(v => v.Key);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public override async Task<string> GetSecretsAsJson(string vaultBaseUrl) =>
			ToJson(await GetSecretsAsDictionary(vaultBaseUrl));

		public override Task SetSecretsFromJson(string vaultBaseUrl, string json) =>
			SetSecretsFromDictionary(vaultBaseUrl, FromJson(json));

		public override async Task<Dictionary<string, string>> GetSecretsAsDictionary(string vaultBaseUrl)
		{
			var azureServiceTokenProvider = new AzureServiceTokenProvider();
			var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

			var secretsDict = new Dictionary<string, string>();

			var secrets = await keyVaultClient.GetSecretsAsync(vaultBaseUrl);
			foreach (var secret in secrets)
			{
				var key = secret.Identifier.Name;
				var value = (await keyVaultClient.GetSecretAsync(secret.Identifier.Identifier)).Value;
				secretsDict.Add(key, value);
			}

			var stopwatch = Stopwatch.StartNew();
			while (!string.IsNullOrEmpty(secrets.NextPageLink))
			{
				secrets = await keyVaultClient.GetSecretsNextAsync(secrets.NextPageLink);
				var tasks = secrets.Select(s => keyVaultClient.GetSecretAsync(s.Identifier.Identifier));
				var results = await Task.WhenAll(tasks);
				foreach (var secretValue in results)
				{
					secretsDict.Add(secretValue.SecretIdentifier.Name, secretValue.Value);
				}
				/*
				foreach (var secret in secrets)
				{
					var key = secret.Identifier.Name;
					var value = (await keyVaultClient.GetSecretAsync(secret.Identifier.Identifier)).Value;
					secretsDict.Add(key, value);
				}
				*/
			}
			stopwatch.Stop();
			var time = stopwatch.Elapsed.TotalSeconds;

			return secretsDict;
		}

		public override async Task SetSecretsFromDictionary(string vaultBaseUrl, Dictionary<string, string> secrets)
		{
			var azureServiceTokenProvider = new AzureServiceTokenProvider();
			var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
			foreach (var secret in secrets)
			{
				Console.WriteLine($"Updating secret: '{secret.Key}'");
				await keyVaultClient.SetSecretAsync(vaultBaseUrl, secret.Key, secrets[secret.Key], contentType: "text/plain");
			}
		}

		/*
		private static async Task<string> GetToken(string authority, string resource, string scope)
		{
			const string CLIENTID = "";
			const string CLIENTSECRET = "";
			var authContext = new AuthenticationContext(authority);
			ClientCredential clientCred = new ClientCredential(CLIENTID, CLIENTSECRET);
			AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

			if (result == null)
				throw new InvalidOperationException("Failed to obtain the JWT token");

			return result.AccessToken;
		}
		*/
	}
}
