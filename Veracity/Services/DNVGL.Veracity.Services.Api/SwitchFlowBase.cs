using DNVGL.OAuth.Api.HttpClient;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DNVGL.Veracity.Services.Api
{
	public abstract class SwitchFlowBase : ISwtichFlow
	{
		private readonly object _syncObj = new object();

		protected readonly IEnumerable<OAuthHttpClientOptions> _optionsList;
		
		protected IApiResourceClient Client;
		protected OAuthHttpClientOptions CurrentOptions;


		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ISerializer _serializer;

		public SwitchFlowBase(IEnumerable<OAuthHttpClientOptions> optionsList, IHttpClientFactory httpClientFactory, ISerializer serializer)
		{			
			_optionsList = optionsList;

			_httpClientFactory = httpClientFactory;
			_serializer = serializer;

			CurrentOptions = optionsList.First(); //get the 1st item by default 
			Client = ApiResourceClientBuilder.CreateWithOAuthClientOptions(CurrentOptions).WithHttpFactory(httpClientFactory).WithSerializer(serializer).WithDataFormat(DataFormat.Json).Build();
		}	

		public virtual void Switch(OAuthCredentialFlow flow)
		{
			if (CurrentOptions.Flow != flow)
			{
				lock (_syncObj)
				{
					if (CurrentOptions.Flow != flow)
					{
						CurrentOptions = _optionsList.First(o => o.Flow == flow);
						Client = ApiResourceClientBuilder.CreateWithOAuthClientOptions(CurrentOptions).WithHttpFactory(_httpClientFactory).WithSerializer(_serializer).WithDataFormat(DataFormat.Json).Build();
					}
				}
			}
		}
	}
}
