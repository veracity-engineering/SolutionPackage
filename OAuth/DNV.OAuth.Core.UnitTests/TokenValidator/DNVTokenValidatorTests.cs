using DNV.OAuth.Abstractions.Constants;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Cryptography;
using Xunit;
using CustomValidator = System.Func<System.Collections.Generic.IEnumerable<System.Security.Claims.Claim>, (bool Succeeded, string FailureReason)>;

namespace DNV.OAuth.Core.TokenValidator.UnitTests
{
	public class DNVTokenValidatorTests
	{
		private static readonly string PrivateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIICXQIBAAKBgQDMdprFFMjVC1X8KfsykccZCS4mIW3nmymIcus+cqHhouBspPPi
nfaYOaFr1e4FunVitWag02krJSvD0HoMbK1Gu+flZbZd+HbtZWGP9jl65T71+oc2
NYAZF7Od+SMufRLnmsO9+DXHpr8xkv6a555fXfVeQ7U5HZ37LZEM2jn9qQIDAQAB
AoGBAMU/RmAKhRTCMtlpxqQqbmCAsrymU1i0H4U7GCbOf80lTEyDdaSRr2t7bXaS
k2WDU+s+BRvx1+t/mJD3dka2MRHpOId9SO6h/AmItTZH12l0r/R34GQWT4126f/c
uzQT/x+bBqqo2GSXLINgKMrPQLEjQ4uX8b4lvdKd0lVuaBWBAkEA94oNeff5jC0h
inMt5cPxPaReYM7rG+4XNt+Ud5CXcnVDpDPP0ggmJDAc+8eVW4h3GKlvD4Ns+DEj
Poi82SAAZQJBANNzpBGkPEbMjTiE2f6F28D2MbnVdpODrKi55rBmb3Xz0rJ6yYPQ
OHZHD52coDCotbCHtCNDjnCoUK3e3nb02fUCQQDfDmBsar5MyRIzPuy54WHN6QI+
e9Yx+c6jXL7dKsa9ldtY6HM5UKyF6XEElGkK7SJcb7krIoOb9jNLE04Q3RxdAkBz
HqJUeHXYlcTM4IdOatIZW+/2nKDR7v4xXgSaRSqprPUo9nB/sv0KZ+W4iW0tEKza
Twdjq4QBEaHbB1tWqDeBAkBKiAc5sTBWPZ+4olx4btPYK0SP2Q61SQqJFTQ8IZDN
W+4/J34OaGzFEc2DSosOs9Qmql4oCtcycSm27JUAl/wU
-----END RSA PRIVATE KEY-----";
		private static readonly string PublicKey = @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDMdprFFMjVC1X8KfsykccZCS4m
IW3nmymIcus+cqHhouBspPPinfaYOaFr1e4FunVitWag02krJSvD0HoMbK1Gu+fl
ZbZd+HbtZWGP9jl65T71+oc2NYAZF7Od+SMufRLnmsO9+DXHpr8xkv6a555fXfVe
Q7U5HZ37LZEM2jn9qQIDAQAB
-----END PUBLIC KEY-----";

		[Theory]
		[InlineData("1.0", "appid")]
		[InlineData("2.0", "azp")]
		public void ValidateTokenTest(string ver, string appKey)
		{
			var appId = Guid.NewGuid().ToString();
			var sub = Guid.NewGuid().ToString();
			var aud = Guid.NewGuid().ToString();
			var claims = new Dictionary<string, string>
			{
				{ "ver", ver },
				{ "sub", sub },
				{ appKey, appId }
			};
			var token = CreateToken(aud, claims);
			var parameters = CreateParameters(aud);

			var sut = CreateSUT();
			var user = sut.ValidateToken(token, parameters, out var _);
			Assert.NotNull(user);
			Assert.Equal(sub, user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
			Assert.Equal(FlowTypeClaimValues.ClientFlow, user.FindFirst(TokenClaimTypes.FlowType)?.Value);
			Assert.Equal(appId, user.FindFirst(TokenClaimTypes.RequestParty)?.Value);

			claims = new Dictionary<string, string>
			{
				{ "ver", ver },
				{ "sub", sub },
				{ "azp", aud }
			};
			token = CreateToken(aud, claims);
			user = sut.ValidateToken(token, parameters, out var _);
			Assert.NotNull(user);
			Assert.Equal(sub, user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
			Assert.Equal(FlowTypeClaimValues.UserFlow, user.FindFirst(TokenClaimTypes.FlowType)?.Value);
			Assert.Equal(sub, user.FindFirst(TokenClaimTypes.RequestParty)?.Value);
		}

		[Theory]
		[InlineData("1.0")]
		[InlineData("2.0")]
		[InlineData("3.0")]
		public void ValidateTokenTest_Exceptions(string ver)
		{
			CustomValidator customValidator = claims =>
			{
				var ver = claims.FirstOrDefault(c => c.Type == TokenClaimTypes.Version)?.Value;

				return ver switch
				{
					"1.0" => (false, "ver 1.0 not supported"),
					"2.0" => (true, string.Empty),
					_ => throw new ArgumentException(nameof(ver)),
				};
			};

			var aud = Guid.NewGuid().ToString();
			var claims = new Dictionary<string, string>
			{
				{ "ver", ver }
			};
			var token = CreateToken(aud, claims);
			var parameters = CreateParameters(aud);
			var sut = CreateSUT(customValidator);
			var func = () => sut.ValidateToken(token, parameters, out var _);

			if (ver == "2.0")
			{
				Assert.NotNull(func());
			}
			else
			{
				Assert.Throws<SecurityTokenValidationException>(func);
			}
		}

		private DNVTokenValidator CreateSUT(CustomValidator? customValidator = null)
		{
			var sut = new DNVTokenValidator(customValidator);
			return sut;
		}

		private string CreateToken(string audience, IDictionary<string, string>? claims)
		{
			var now = DateTime.UtcNow;
			var key = CreateRsaSecurityKey(PrivateKey);
			var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
			var token = new JwtSecurityToken(
				null,
				audience,
				claims?.Select(c => new Claim(c.Key, c.Value)),
				now,
				now.AddHours(1),
				signingCredentials
			);

			string jwt = new JwtSecurityTokenHandler().WriteToken(token);
			return jwt;
		}

		private TokenValidationParameters CreateParameters(string audience)
		{
			return new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = true,
				ValidAudience = audience,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = CreateRsaSecurityKey(PublicKey)
			};
		}

		private RsaSecurityKey CreateRsaSecurityKey(string pemKey)
		{
			var rsa = RSA.Create();
			rsa.ImportFromPem(pemKey.ToCharArray());
			return new RsaSecurityKey(rsa);
		}
	}
}