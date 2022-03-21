using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DNVGL.OAuth.Api.HttpClient;

[JsonConverter(typeof(StringEnumConverter))]
public enum OAuthCredentialFlow
{
	[EnumMember(Value = "user-credentials")]
	UserCredentials = 0,
	[EnumMember(Value = "client-credentials")]
	ClientCredentials = 1
}