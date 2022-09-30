using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DNV.Security.DataProtection
{
	public static class DataProtectionBuilderExtensions
	{
		public static IDataProtectionBuilder ProtectKeysWithPasskey(this IDataProtectionBuilder builder, string passkey)
		{
			var services = builder.Services;

			services.AddSingleton<PasskeyXmlCipher>()
				.Configure<PasskeyXmlCipherOptions>(o => o.Passkey = passkey)
				.AddSingleton<IConfigureOptions<KeyManagementOptions>>(
					p =>
					{
						var xmlEncryptor = p.GetRequiredService<PasskeyXmlCipher>();
						return new ConfigureOptions<KeyManagementOptions>(o => o.XmlEncryptor = xmlEncryptor);
					}
				);
			return builder;
		}
	}
}
