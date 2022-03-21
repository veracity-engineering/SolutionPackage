using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using DNV.OAuth.Abstractions;

namespace DNVGL.OAuth.Api.HttpClient
{
	[Obsolete("Please use class OAuthHttpClientOptions in BCL instead.")]
    public class OAuthHttpClientFactoryOptions: OAuthHttpClientOptions
    {
    }
}
