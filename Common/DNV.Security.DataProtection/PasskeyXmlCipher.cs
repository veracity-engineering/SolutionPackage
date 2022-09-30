using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace DNV.Security.DataProtection
{
	public class PasskeyXmlCipher : IXmlEncryptor, IXmlDecryptor
	{
		private readonly SymmetricAlgorithm _cipher;

		public PasskeyXmlCipher(IOptions<PasskeyXmlCipherOptions> options) : this(options.Value) { }

		public PasskeyXmlCipher(PasskeyXmlCipherOptions? options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));

			if (string.IsNullOrEmpty(options.Passkey))
			{
				throw new ArgumentNullException(nameof(options.Passkey));
			}

			_cipher = CreateCipher(options.Passkey);
		}

		public EncryptedXmlInfo Encrypt(XElement plaintextElement)
		{
			using var ms = new MemoryStream();
			plaintextElement.Save(ms, SaveOptions.DisableFormatting);
			var plaintextData = ms.ToArray();
			var encryptedData = _cipher.CreateEncryptor().TransformFinalBlock(plaintextData, 0, plaintextData.Length);
			var element = new XElement(
				"encryptedKey",
				new XComment("This key is encrypted with Passkey"),
				new XElement("value", Convert.ToBase64String(encryptedData))
			);
			return new EncryptedXmlInfo(element, typeof(PasskeyXmlCipher));
		}

		public XElement Decrypt(XElement encryptedElement)
		{
			var encryptedData = Convert.FromBase64String(encryptedElement.Element((XName?)"value")!.Value);
			var decryptedData = _cipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
			using var ms = new MemoryStream(decryptedData);
			return XElement.Load(ms);
		}

		private static SymmetricAlgorithm CreateCipher(string passkey)
		{
			var bytes = Encoding.UTF8.GetBytes(passkey);
			var aes = Aes.Create();
			aes.KeySize = 256;
			aes.Mode = CipherMode.CBC;
			aes.Key = SHA256.Create().ComputeHash(bytes);
			aes.IV = MD5.Create().ComputeHash(bytes);
			return aes;
		}
	}
}