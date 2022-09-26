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
				.AddSingleton(new PasskeyXmlCipherOptions { Passkey = passkey })
				.AddSingleton(p =>
					new ConfigureOptions<KeyManagementOptions>(o =>
						o.XmlEncryptor = p.GetRequiredService<PasskeyXmlCipher>()
					)
				);
			return builder;
		}
	}
}
