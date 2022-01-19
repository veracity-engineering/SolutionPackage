using System;
using System.Collections.Generic;
using System.Linq;

namespace DNVGL.Web.Security.CSP
{
	public class ContentSecurityPolicy
	{
		public static readonly string Name = "Content-Security-Policy";

		private readonly Dictionary<string, Directive> _directives = new Dictionary<string, Directive>();

		public void SetDirective(string directive, string value)
		{
			this.SetDirective(directive, new Directive(value));

		}

		public void SetDirective(string directive, Directive values)
		{
			if (string.IsNullOrWhiteSpace(directive)) throw new ArgumentNullException(nameof(directive));

			_directives[directive] = values;
		}

		public Directive GetDirective(string directive) => _directives[directive];

		public string GetValue()
		{
			return string.Join(";", _directives.Select(p => $"{p.Key} {string.Join(" ", p.Value)}"));
		}

		public static ContentSecurityPolicy CreateDefault(string nonce = null)
		{
			var csp = new ContentSecurityPolicy();
			csp.SetDirective(DirectiveNames.ConnectSrc, "'self' https://dc.services.visualstudio.com https://login.microsoftonline.com https://login.veracity.com https://loginstag.veracity.com https://logintest.veracity.com");
			csp.SetDirective(DirectiveNames.DefaultSrc, "'self'");
			csp.SetDirective(DirectiveNames.FontSrc, "'self' data: https://onedesign.azureedge.net");
			csp.SetDirective(DirectiveNames.FrameSrc, "'self' https://www.google.com https://www.recaptcha.net/");
			csp.SetDirective(DirectiveNames.ImgSrc, "'self' https://onedesign.azureedge.net");
			csp.SetDirective(DirectiveNames.MediaSrc, "'self'");
			csp.SetDirective(DirectiveNames.ObjectSrc, $"'self' {nonce}");
			csp.SetDirective(DirectiveNames.ScriptSrc, $"'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn {nonce}");
			csp.SetDirective(DirectiveNames.StyleSrc, $"'self' https://onedesign.azureedge.net {nonce}");
			csp.SetDirective(DirectiveNames.WorkerSrc, "'self' blob:");
			return csp;
		}
	}
}
