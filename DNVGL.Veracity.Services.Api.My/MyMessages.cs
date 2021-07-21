using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyMessages : ApiResourceClient, IMyMessages
    {
        public MyMessages(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

		public Task<IEnumerable<Message>> List(bool includeRead = false) =>
			GetResult<IEnumerable<Message>>(MyMessagesUrls.List(includeRead), false);

		public Task<Message> Get(string messageId) =>
			GetResult<Message>(MyMessagesUrls.Message(messageId));

		public Task<int> GetUnreadCount() =>
			GetResult<int>(MyMessagesUrls.UnreadCount, false);
    }

    internal static class MyMessagesUrls
    {
        public static string Root => "/veracity/services/v3/my/messages";

        public static string List(bool includeRead) => includeRead
            ? $"{Root}?all=true"
            : Root;

        public static string Message(string messageId) => $"{Root}/{messageId}";

        public static string UnreadCount => $"{Root}/count";
    }
}
