using System;

namespace DNVGL.OAuth.Web.Extensions.Veracity
{
	[Flags]
	public enum PolicyValidationMode
	{
		PlatformTermsAndCondition = 0x00000001,
        PlatformAndService = 0x00000002,
        ServiceSubscription = 0x00000004,
        All = PlatformAndService | ServiceSubscription
    }

    public class PolicyValidationOptions
    {
        public PolicyValidationMode PolicyValidationMode { get; set; }

        public string MyPoliciesApiConfigName { get; set; }

        public string ServiceId { get; set; }
    }
}
