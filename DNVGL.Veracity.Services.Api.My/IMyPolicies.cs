﻿using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public interface IMyPolicies
    {
        Task ValidatePolicy(string serviceId, string returnUrl, string skipSubscriptionCheck);

        Task ValidatePolicies(string returnUrl);
    }
}
