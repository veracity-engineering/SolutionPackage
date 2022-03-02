using System;
using System.Net.Http;

namespace DNVGL.OAuth.Api.HttpClient
{
    /// <summary>
	/// 
	/// </summary>
    public interface IDelegatingHandlerBuilder
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
        DelegatingHandler BuildHandler(OAuthHttpClientFactoryOptions option);
    }
}
