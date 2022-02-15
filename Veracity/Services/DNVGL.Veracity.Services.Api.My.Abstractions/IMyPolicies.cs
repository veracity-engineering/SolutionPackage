using System.Threading.Tasks;
using DNVGL.Veracity.Services.Api.Models;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
    public interface IMyPolicies
    {
        Task<PolicyValidationResult> ValidatePolicy(string serviceId, string returnUrl = null, string skipSubscriptionCheck = null);

        Task<PolicyValidationResult> ValidatePolicies(string returnUrl = null);
    }
}
