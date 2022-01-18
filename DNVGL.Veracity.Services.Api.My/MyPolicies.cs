﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Models;

namespace DNVGL.Veracity.Services.Api.My
{
	public class MyPolicies : ApiResourceClient, IMyPolicies
	{
		public MyPolicies(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
		{
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
			return await ToResourceResult<PolicyValidationResult>(request);
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
			return await ToResourceResult<PolicyValidationResult>(request);
		}
	}

	internal static class MyPoliciesUrls
	{
		public static string Root => "/veracity/services/v3/my/policies";

		public static string ValidatePolicies => $"{Root}/validate()";

		public static string ValidatePolicy(string serviceId) => $"{Root}/{serviceId}/validate()";
	}
}
