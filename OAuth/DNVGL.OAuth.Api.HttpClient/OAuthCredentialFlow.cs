using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DNVGL.OAuth.Api.HttpClient;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OAuthCredentialFlow
{
	[EnumMember(Value = "user-credentials")]
	UserCredentials = 0,
	[EnumMember(Value = "client-credentials")]
	ClientCredentials = 1
}