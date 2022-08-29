using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
	internal class MyWidgets : ApiClientBase, IMyWidgets
	{	
		public MyWidgets(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
			: base(optionsList, httpClientFactory, serializer)
		{

		}

		public Task<IEnumerable<Widget>> Get()=>
			base.GetClient().GetResource<IEnumerable<Widget>>(MyWidgetsUrls.Root);

		internal static class MyWidgetsUrls
		{
			public static string Root => "/Veracity/Services/V3/my/widgets";
		}
	}
}
