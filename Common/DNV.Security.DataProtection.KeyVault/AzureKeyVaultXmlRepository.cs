using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;

namespace DNV.Security.DataProtection.KeyVault
{
	public class AzureKeyVaultXmlRepository : IXmlRepository
	{
		private readonly Uri _vaultUri;

		private readonly string _secretName;

		private readonly TokenCredential _tokenCredential;

		public AzureKeyVaultXmlRepository(Uri vaultUri, string secretName, TokenCredential tokenCredential)
		{
			_vaultUri = vaultUri;
			_secretName = secretName;
			_tokenCredential = tokenCredential;
		}

		public IReadOnlyCollection<XElement> GetAllElements()
		{
			var client = new SecretClient(_vaultUri, _tokenCredential);

			try
			{
				var secret = client.GetSecret(_secretName).Value;
				var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(secret.Value));
				var xml = XDocument.Parse(decoded);
				var entries = new List<XElement>();

				foreach (XElement node in xml.Root!.Elements())
				{
					string text = node.Name.ToString();
					string text2 = text;

					if (!(text2 == "keys"))
					{
						if (text2 == "revocation")
						{
							var revocationDate = DateTime.Parse(node.Element("revocationDate")!.Value);
							var revocationAge = (DateTime.Now - revocationDate).Days;

							if (revocationAge < 14) entries.Add(node);
						}
						else entries.Add(node);
					}
					else
					{
						var created = DateTime.Parse(node.Element("creationDate")!.Value);
						var keyAge = (DateTime.Now - created).Days;

						if (keyAge <= 14) entries.Add(node);
					}
				}

				return entries;
			}
			catch { }

			return new List<XElement>();
		}

		public void StoreElement(XElement element, string friendlyName)
		{
			var keys = (IList<XElement>)GetAllElements();
			keys.Add(element);
			var xml = new XDocument(new XElement("keys"));

			foreach (XElement key in keys)
			{
				xml.Root!.Add(key);
			}

			var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.ToString()));
			var client = new SecretClient(_vaultUri, _tokenCredential);
			var keyVaultSecret = new KeyVaultSecret(_secretName, encoded);
			keyVaultSecret.Properties.ContentType = "text/plain";
			var secret = keyVaultSecret;
			client.SetSecret(secret);
		}
	}

}