using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
    public interface IMyPolicies
    {
        Task ValidatePolicy(string serviceId, string returnUrl = null, string skipSubscriptionCheck = null);

        Task ValidatePolicies(string returnUrl = null);
    }
}
