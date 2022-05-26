using System;

namespace DNV.OAuth.Web.Extensions.Policy
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum PolicyValidationMode
	{
		/// <summary>
		/// 
		/// </summary>
		PlatformTermsAndCondition = 0x00000001,

		/// <summary>
		/// 
		/// </summary>
		PlatformAndService = 0x00000002,

		/// <summary>
		/// must be used together with PlatformTermsAndCondition or PlatformAndService
		/// </summary>
		ServiceSubscription = 0x00000004,
		
		/// <summary>
		/// 
		/// </summary>
		All = PlatformAndService | ServiceSubscription
	}
}