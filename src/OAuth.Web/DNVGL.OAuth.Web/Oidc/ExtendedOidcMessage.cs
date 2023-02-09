using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DNVGL.OAuth.Web.Oidc
{
#if NETCORE2
	internal class ExtendedOidcMessage: OpenIdConnectMessage
	{
		public ExtendedOidcMessage(OpenIdConnectMessage other) : base(other) { }

		public override string BuildFormPost()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<html><head><title>");
			stringBuilder.Append(WebUtility.HtmlEncode(this.PostTitle));
			stringBuilder.Append("</title></head><body><form method=\"POST\" name=\"hiddenform\" action=\"");
			stringBuilder.Append(WebUtility.HtmlEncode(this.IssuerAddress));
			stringBuilder.Append("\">");
			foreach (KeyValuePair<string, string> parameter in Parameters)
			{
				stringBuilder.Append("<input type=\"hidden\" name=\"");
				stringBuilder.Append(WebUtility.HtmlEncode(parameter.Key));
				stringBuilder.Append("\" value=\"");
				stringBuilder.Append(WebUtility.HtmlEncode(parameter.Value));
				stringBuilder.Append("\" />");
			}
			stringBuilder.Append("<noscript><p>");
			stringBuilder.Append(WebUtility.HtmlEncode(this.ScriptDisabledText));
			stringBuilder.Append("</p><input type=\"submit\" value=\"");
			stringBuilder.Append(WebUtility.HtmlEncode(this.ScriptButtonText));
			stringBuilder.Append("\" /></noscript>");
			stringBuilder.Append($"</form>{OidcMessageExtensions.FormPostScript}</body></html>");
			return stringBuilder.ToString();
        }
	}
#endif
}
