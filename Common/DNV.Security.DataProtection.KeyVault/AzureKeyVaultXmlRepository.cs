using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Azure;
using System.Net;

namespace DNV.Security.DataProtection.KeyVault
{
	public class AzureKeyVaultXmlRepository : IXmlRepository
	{
		private const int MaximumKeyAge = 90;

		private readonly ILogger _logger;
		private readonly SecretClient _secretClient;
		private readonly Uri _vaultUri;
		private readonly string _secretName;
		private readonly TokenCredential _tokenCredential;

		public AzureKeyVaultXmlRepository(
			ILoggerFactory loggerFactory,
			IAzureClientFactory<SecretClient> secretClientFactory,
			Uri vaultUri,
			string secretName,
			TokenCredential tokenCredential
		)
		{
			_logger = loggerFactory.CreateLogger<AzureKeyVaultXmlRepository>();
			_secretClient = secretClientFactory.CreateClient(nameof(AzureKeyVaultXmlRepository));
			_vaultUri = vaultUri;
			_secretName = secretName;
			_tokenCredential = tokenCredential;
		}

		public IReadOnlyCollection<XElement> GetAllElements()
		{
			try
			{
				var secret = _secretClient.GetSecret(_secretName).Value;
				var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(secret.Value));
				var xml = XDocument.Parse(decoded);
				var entries = new List<XElement>();

				foreach (XElement node in xml.Root.Elements())
				{
					var name = node.Name.ToString();

					switch (name)
					{
						case "key":
							var created = DateTime.Parse(node.Element("creationDate").Value);
							var keyAge = (DateTime.Now - created).Days;

							if (keyAge <= MaximumKeyAge) entries.Add(node);
							else
							{
								var keyId = node.Attribute("id").Value;
								_logger.LogCritical("Skip retired key {keyId}", keyId);
							}

							break;
						case "revocation":
							var revocationDate = DateTime.Parse(node.Element("revocationDate").Value);
							var revocationAge = (DateTime.Now - revocationDate).Days;

							if (revocationAge < MaximumKeyAge) entries.Add(node);

							break;
						default:
							_logger.LogCritical("Unexpected element {name}", name);
							break;
					}
				}

				return entries;
			}
			catch (RequestFailedException ex)
			{
				if (ex.Status == (int)HttpStatusCode.NotFound)
				{
					_logger.LogWarning("Keyring file '{secretName}' was not found, a new one will be created", _secretName, ex.Message);
				}
				else
				{
					_logger.LogCritical("Unexpected error", ex.Message);
				}
			}
			catch (Exception ex)
			{
				_logger.LogCritical("Unexpected error", ex.Message);
			}

			return new List<XElement>();
		}

		public void StoreElement(XElement element, string friendlyName)
		{
			var keys = (IList<XElement>)GetAllElements();
			keys.Add(element);
			var xml = new XDocument(new XElement("keys", keys));
			var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.ToString()));
			_secretClient.SetSecret(_secretName, encoded);
		}
	}

}