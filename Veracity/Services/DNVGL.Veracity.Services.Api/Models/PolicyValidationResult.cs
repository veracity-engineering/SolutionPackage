using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Veracity.Services.Api.Models
{
	public class PolicyValidationResult
	{
		public string Url { get; set; }
		public List<string> ViolatedPolicies { get; set; }
		public string Message { get; set; }
		public string Information { get; set; }
		public int SubCode { get; set; }
		public int StatusCode { get; set; }
    }
}
