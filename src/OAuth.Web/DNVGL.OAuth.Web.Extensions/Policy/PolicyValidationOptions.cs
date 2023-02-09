using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Policy
{
	/// <summary>
	/// 
	/// </summary>
	public class PolicyValidationOptions
    {
        /// <summary>
		/// 
		/// </summary>
	    public const string VeracityDefaultPolicy = nameof(VeracityDefaultPolicy);

        /// <summary>
		/// 
		/// </summary>
        public PolicyValidationMode PolicyValidationMode { get; set; }

        /// <summary>
		/// 
		/// </summary>
        public string? VeracityPolicyApiConfigName { get; set; }

        /// <summary>
		/// 
		/// </summary>
        public string? ServiceId { get; set; }

        /// <summary>
		/// 
		/// </summary>
        public Func<HttpContext, string, string>? GetReturnUrl { get; set; }

        /// <summary>
		/// 
		/// </summary>
        public string AuthorizationPolicyName { get; set; } = VeracityDefaultPolicy;

		/// <summary>
		/// 
		/// </summary>
        public bool AddAsDefaultPolicy { get; set; } = true;
    }
}
