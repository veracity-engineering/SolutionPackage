using DNVGL.OAuth.Api.HttpClient;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DNVGL.Veracity.Services.Api
{
	public class ApiResourceClientBuilder
	{
		private readonly ApiClientConfiguration _config; 

		internal ApiResourceClientBuilder(ApiClientConfiguration config)
		{
			_config = config;
		}

		public static ApiResourceClientBuilder CreateWithOAuthClientOptions(OAuthHttpClientOptions options)
		{
			var config = new ApiClientConfiguration
			{
				OAuthClientOptions = options
			};
			
			return new ApiResourceClientBuilder(config);
		}

		public ApiResourceClientBuilder WithHttpFactory(IHttpClientFactory httpClientFactory)
		{
			_config.HttpClientFactory = httpClientFactory;
			return this;
		}

		public ApiResourceClientBuilder WithSerializer(ISerializer serializer)
		{
			_config.Serializer = serializer;
			return this;
		}

		public ApiResourceClientBuilder WithDataFormat(DataFormat dataFormat)
		{
			_config.AccepHeaderDataFormat = dataFormat;
			return this;
		}

		public ApiResourceClientBuilder WithTimeout(TimeSpan timeout)
		{
			_config.Timeout = timeout;
			return this;
		}

		public IApiClient Build()
		{
			if (_config.HttpClientFactory == null || _config.Serializer == null || _config.OAuthClientOptions == null)
				throw new ArgumentNullException("Missing httpclientfactory, serializer or oauthclientoptions!");

			var client = _config.HttpClientFactory.CreateClient(_config.OAuthClientOptions.GetHttpClientName());
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ToAcceptMediaType(_config.AccepHeaderDataFormat ?? _config.Serializer.DataFormat)));

			if (_config.Timeout != null)
			{
				client.Timeout = _config.Timeout.Value;
			}

			return new ApiClient(client, _config.Serializer);			
		}

		protected string ToAcceptMediaType(DataFormat dataFormat)
		{
			switch (dataFormat)
			{
				case DataFormat.Json:
					return "application/json";
				case DataFormat.Xml:
					return "application/xml";
				default:
					throw new NotImplementedException($"Unknown {nameof(DataFormat)} type.");
			}
		}

	}
}
