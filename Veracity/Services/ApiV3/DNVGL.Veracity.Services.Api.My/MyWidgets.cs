using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    internal class MyWidgets :  IMyWidgets
	{
        private readonly ApiClientFactory _apiClientFactory;
        public MyWidgets(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        public Task<IEnumerable<Widget>> Get()=>
            _apiClientFactory.GetClient().GetResource<IEnumerable<Widget>>(MyWidgetsUrls.Root);

		internal static class MyWidgetsUrls
		{
			public static string Root => "/Veracity/Services/V3/my/widgets";
		}
	}
}
