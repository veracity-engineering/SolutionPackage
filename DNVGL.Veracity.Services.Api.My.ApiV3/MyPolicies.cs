using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using System;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.ApiV3
{
    public class MyPolicies : ApiResourceClient, IMyPolicies
    {
        public MyPolicies(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public Task ValidatePolicies(string returnUrl)
        {
            throw new NotImplementedException();
        }

        public Task ValidatePolicy(string serviceId, string returnUrl, string skipSubscriptionCheck)
        {
            throw new NotImplementedException();
        }
    }

    internal class MyPoliciesUrls
    {
        public static string Root => "/veracity/services/v3/my/policies";

        public static string ValidatePolicies => $"{Root}/validate()";

        public static string ValidatePolicy(string serviceId) => $"{Root}/{serviceId}/validate()";
    }
}
