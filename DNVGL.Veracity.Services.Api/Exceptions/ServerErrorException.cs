using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.Exceptions
{
	[Serializable]
	public class ServerErrorException : Exception
	{
		public HttpStatusCode StatusCode { get; set; }

		public string ResponseContent { get; set; }

		public ServerErrorException(HttpStatusCode httpStatusCode, string responseContent, string message, Exception innerException): base(message, innerException)
		{
			StatusCode = httpStatusCode;
			ResponseContent = responseContent;
		}

		protected ServerErrorException(SerializationInfo info, StreamingContext context): base(info, context)
		{
		}

		public static async Task<ServerErrorException> FromResponse(HttpResponseMessage response, Exception innerException) =>
			new ServerErrorException(response.StatusCode, await response.Content?.ReadAsStringAsync(), "The status of the response did not indicate success.", innerException);
	}
}
