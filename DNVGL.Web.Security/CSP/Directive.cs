using System;
using System.Collections.Generic;

namespace DNVGL.Web.Security.CSP
{
	public class Directive : HashSet<string>
	{
		public Directive() : base() { }

		public Directive(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				var values = value.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var v in values) this.Add(v);
			}
		}
	}
}
