using System;
using System.Threading.Tasks;
using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace DNV.OAuth.Web.Extensions.Mfa
{
	public static class MfaExtensions
	{
		private const string MfaRequiredParam = "mfa_required";
		private const string MfaType = "mfaType";
		private const string MfaForPhone = "phone";
		private const string MfaForFederatedIdp = "federatedIdp";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="mfaPredict"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static OidcOptions AddMfaSupport(this OidcOptions options, Func<HttpRequest, bool> mfaPredict = null)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			options.Events = options.Events ?? new OpenIdConnectEvents();

			var previous = options.Events.OnRedirectToIdentityProvider;
			options.Events.OnRedirectToIdentityProvider = async ctx =>
			{
				if (previous != null)
					await previous(ctx).ConfigureAwait(false);
				SetMfaParameter(ctx, mfaPredict);
			};

			return options;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="mfaPredict"></param>
		private static void SetMfaParameter(RedirectContext ctx, Func<HttpRequest, bool> mfaPredict)
		{
			if (mfaPredict?.Invoke(ctx.Request)
				?? ctx.Properties.GetParameter<bool>(MfaRequiredParam))
				ctx.ProtocolMessage.SetParameter(MfaRequiredParam, "true");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="redirectUrl"></param>
		/// <returns></returns>
		public static Task ChallengeForMfaAsync(this HttpContext ctx, string redirectUrl)
		{
			if (ctx == null)
				throw new ArgumentNullException(nameof(ctx));

			if (string.IsNullOrEmpty(redirectUrl))
				throw new ArgumentNullException(nameof(redirectUrl));

			var properties = new AuthenticationProperties
			{
				RedirectUri = redirectUrl
			};

			properties.SetParameter(MfaRequiredParam, true);

			return ctx.ChallengeAsync(properties);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public static bool SignedInWithMfa(this HttpContext ctx)
		{
			if (ctx == null)
				throw new ArgumentNullException(nameof(ctx));

			return ctx.User?.FindFirst(c =>
				c.Type == MfaType
				&& (c.Value == MfaForPhone || c.Value == MfaForFederatedIdp)) != null;
		}
	}
}
