using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.SecretsManager.Services
{
	public class KeyVaultSecretsService : SecretsService
	{
		public override Task<IEnumerable<KeyValuePair<string, string>>> GetSources()
		{
			var sources = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>("StoreWebKVDevTest","https://storewebkvdevtest.vault.azure.net"),
				new KeyValuePair<string, string>("StoreWebKVTest","https://storewebkvtest.vault.azure.net"),
				new KeyValuePair<string, string>("StoreWebKVStag","https://storewebkvstag.vault.azure.net")
			};
			return Task.FromResult((IEnumerable<KeyValuePair<string, string>>)sources);
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
			var nextPageLink = secrets.NextPageLink;
			foreach (var secret in secrets)
			{
				var key = secret.Identifier.Name;
				var value = (await keyVaultClient.GetSecretAsync(secret.Identifier.Identifier)).Value;
				secretsDict.Add(key, value);
			}

			var stopwatch = Stopwatch.StartNew();
			while (!string.IsNullOrEmpty(nextPageLink))
			{
				secrets = await keyVaultClient.GetSecretsNextAsync(nextPageLink);
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
				nextPageLink = secrets.NextPageLink;
			}
			stopwatch.Stop();
			var time = stopwatch.Elapsed.TotalSeconds;

			return secretsDict;
		}

		public override Task SetSecretsFromDictionary(string vaultBaseUrl, Dictionary<string, string> secrets)
		{
			var azureServiceTokenProvider = new AzureServiceTokenProvider();
			var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
			var tasks = secrets.Select(s => keyVaultClient.SetSecretAsync(vaultBaseUrl, s.Key, secrets[s.Key], contentType: "text/plain"));
			return Task.WhenAll(tasks);
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
