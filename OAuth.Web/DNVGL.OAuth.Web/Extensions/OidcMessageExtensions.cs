using System.Linq;
using System.Reflection;
using Microsoft.IdentityModel.Protocols;

namespace DNVGL.OAuth.Web.Extensions
{
	public static class OidcMessageExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		public const string FormPostScriptHashCode = "sha256-N4ps+XP2YXr4JI2/sWVoER7gSQH2UxrXbN3v6MvHM4I=";


		internal const string FormPostScript = "<script language=\"javascript\">" +
		                              "window.setTimeout(function() { document.forms[0].submit(); }, 0);" +
		                              "</script>";
#if NETCORE3
		internal static void EnsureCspForOidcFormPostBehavior(this AuthenticationProtocolMessage message)
		{
			var scriptField = typeof(AuthenticationProtocolMessage)
				.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
				.FirstOrDefault(f =>
					f.IsInitOnly && f.Name.Contains(nameof(message.Script)) && f.Name.EndsWith("BackingField"));

			if (scriptField != null)
				scriptField.SetValue(message, FormPostScript);
		}
#endif
	}
}
