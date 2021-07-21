using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Exceptions
{
	public class ServerErrorException : Exception
	{
		public HttpStatusCode StatusCode { get; set; }

		public string ResponseContent { get; set; }

		public ServerErrorException(HttpStatusCode httpStatusCode, string responseContent)
		{
			StatusCode = httpStatusCode;
			ResponseContent = responseContent;
		}

		public static async Task<ServerErrorException> FromResponse(HttpResponseMessage response) =>
			new ServerErrorException(response.StatusCode, await response.Content.ReadAsStringAsync());
	}
}
