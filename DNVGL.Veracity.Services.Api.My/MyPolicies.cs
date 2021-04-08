using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
	public class MyPolicies : ApiResourceClient, IMyPolicies
	{
		public MyPolicies(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
		{
		}

		public async Task ValidatePolicies(string returnUrl = null)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, MyPoliciesUrls.ValidatePolicies);
			if (!string.IsNullOrEmpty(returnUrl))
				request.Headers.Add("returnUrl", returnUrl);
			var response = await GetOrCreateHttpClient().SendAsync(request);
			response.EnsureSuccessStatusCode();
		}

		public async Task ValidatePolicy(string serviceId, string returnUrl = null, string skipSubscriptionCheck = null)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, MyPoliciesUrls.ValidatePolicy(serviceId));
			if (!string.IsNullOrEmpty(returnUrl))
				request.Headers.Add("returnUrl", returnUrl);
			if (!string.IsNullOrEmpty(skipSubscriptionCheck))
				request.Headers.Add("skipSubscriptionCheck", skipSubscriptionCheck);
			var response = await GetOrCreateHttpClient().SendAsync(request);
			response.EnsureSuccessStatusCode();
		}
	}

	internal static class MyPoliciesUrls
	{
		public static string Root => "/veracity/services/v3/my/policies";

		public static string ValidatePolicies => $"{Root}/validate()";

		public static string ValidatePolicy(string serviceId) => $"{Root}/{serviceId}/validate()";
	}
}
