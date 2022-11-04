using DNVGL.OAuth.Api.HttpClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DNVGL.Veracity.Services.Api
{
	internal class ApiClientConfiguration
	{
		public OAuthHttpClientOptions OAuthClientOptions { get; set; }
		
		public DataFormat? AccepHeaderDataFormat { get; set; }

		public IHttpClientFactory HttpClientFactory { get; set; }

		public ISerializer Serializer { get; set; }

		public TimeSpan? Timeout { get; set; }
	}
}
