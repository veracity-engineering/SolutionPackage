using System;

namespace DNV.OAuth.Web.Extensions.Veracity
{
	[Flags]
	public enum PolicyValidationMode
	{
		PlatformTermsAndCondition = 0x00000001,
		PlatformAndService = 0x00000002,
		ServiceSubscription = 0x00000004, // must be used together with PlatformTermsAndCondition or PlatformAndService
		All = PlatformAndService | ServiceSubscription
	}
}