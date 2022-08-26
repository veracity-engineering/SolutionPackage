using System.Net;
using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Exceptions;
using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;

namespace DNVGL.Veracity.Services.Api.My
{
	public class MyPolicies : ApiClientBase, IMyPolicies
	{
		public MyPolicies(IHttpClientFactory httpClientFactory, ISerializer serializer, IEnumerable<OAuthHttpClientOptions> optionsList)
			: base(optionsList, httpClientFactory, serializer)
		{

		}

		protected override async Task CheckResponse(HttpResponseMessage response, bool ignoreNotFound = false)
        {
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotAcceptable)
            {
                if (ignoreNotFound && response.StatusCode == HttpStatusCode.NotFound)
                    return;

                throw await ServerErrorException.FromResponse(response);
            }
        }

        protected override async Task<T> BuildResult<T>(HttpResponseMessage response)
        {
			var result = default (T);
            if (response.StatusCode == HttpStatusCode.NotAcceptable)
                result = await base.GetClient().DeserializeFromStream<T>(
		                await response.Content.ReadAsStreamAsync().ConfigureAwait(false)).ConfigureAwait(false);
            else if (response.IsSuccessStatusCode)
            {
                var r = new PolicyValidationResult { StatusCode = (int)response.StatusCode };
                result = (T)(object)r;
            }

            return result;
        }

		/// <summary>
		/// Validates all policies for the authenticated user.
		/// </summary>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		public async Task<PolicyValidationResult> ValidatePolicies(string returnUrl = null)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, MyPoliciesUrls.ValidatePolicies);
			if (!string.IsNullOrEmpty(returnUrl))
				request.Headers.Add("returnUrl", returnUrl);

			return await base.GetClient().ToResourceResult<PolicyValidationResult>(request, isNotFoundNull: false, buildResult: async resp => { return await BuildResult<PolicyValidationResult>(resp); }, checkResponse: async (resp, ignoreNotFound) => { await CheckResponse(resp, ignoreNotFound); });
		}

		/// <summary>
		/// Validates an individual policy for the authenticated user.
		/// </summary>
		/// <param name="serviceId"></param>
		/// <param name="returnUrl"></param>
		/// <param name="skipSubscriptionCheck"></param>
		/// <returns></returns>
		public async Task<PolicyValidationResult> ValidatePolicy(string serviceId, string returnUrl = null, string skipSubscriptionCheck = null)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, MyPoliciesUrls.ValidatePolicy(serviceId));
			if (!string.IsNullOrEmpty(returnUrl))
				request.Headers.Add("returnUrl", returnUrl);
			if (!string.IsNullOrEmpty(skipSubscriptionCheck))
				request.Headers.Add("skipSubscriptionCheck", skipSubscriptionCheck);
			return await base.GetClient().ToResourceResult<PolicyValidationResult>(request, isNotFoundNull: false, buildResult: async resp => { return await BuildResult<PolicyValidationResult>(resp); }, checkResponse: async (resp, ignoreNotFound) => { await CheckResponse(resp, ignoreNotFound); });
		}
	}

	internal static class MyPoliciesUrls
	{
		public static string Root => "/veracity/services/v3/my/policies";

		public static string ValidatePolicies => $"{Root}/validate()";

		public static string ValidatePolicy(string serviceId) => $"{Root}/{serviceId}/validate()";
	}
}
