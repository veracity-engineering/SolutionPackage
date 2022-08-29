using DNVGL.OAuth.Api.HttpClient;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace DNVGL.Veracity.Services.Api
{
	public abstract class ApiClientBase
	{
		private IApiResourceClient _apiResourceClientForSingleFlow;

		internal protected readonly IEnumerable<OAuthHttpClientOptions> _optionsList;

		protected readonly IHttpClientFactory _httpClientFactory;
		protected readonly ISerializer _serializer;

		public ApiClientBase(IEnumerable<OAuthHttpClientOptions> optionsList, IHttpClientFactory httpClientFactory, ISerializer serializer)
		{	

			if(optionsList == null || !optionsList.Any()) 
			  throw new System.ArgumentException(nameof(optionsList));

			_optionsList = optionsList;

			_httpClientFactory = httpClientFactory;
			_serializer = serializer;		
		}

		public virtual IApiResourceClient GetClient(OAuthCredentialFlow? flow = null)
		{
			if (_optionsList.Count() == 1)
			{
				if (_apiResourceClientForSingleFlow == null)
				{
					Interlocked.CompareExchange(ref _apiResourceClientForSingleFlow, ApiResourceClientBuilder.CreateWithOAuthClientOptions(_optionsList.First()).WithHttpFactory(_httpClientFactory).WithSerializer(_serializer).WithDataFormat(DataFormat.Json).Build(), null);
				}

				return _apiResourceClientForSingleFlow;
			}
			else
			{
				var options = flow == null ? _optionsList.First() : _optionsList.First(o => o.Flow == flow);

				return ApiResourceClientBuilder.CreateWithOAuthClientOptions(options).WithHttpFactory(_httpClientFactory).WithSerializer(_serializer).WithDataFormat(DataFormat.Json).Build();
			}
		}		
	}
}
