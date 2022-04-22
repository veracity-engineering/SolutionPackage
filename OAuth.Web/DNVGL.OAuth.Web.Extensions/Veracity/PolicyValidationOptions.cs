using System;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Veracity
{
	public class PolicyValidationOptions
    {
        public PolicyValidationMode PolicyValidationMode { get; set; }

        public string? VeracityPolicyApiConfigName { get; set; }

        public string? ServiceId { get; set; }

        public Func<HttpContext, string, string>? GetReturnUrl { get; set; }
    }
}
