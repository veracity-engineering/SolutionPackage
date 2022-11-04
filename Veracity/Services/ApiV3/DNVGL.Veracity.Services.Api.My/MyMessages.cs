using DNVGL.Veracity.Services.Api.Extensions;
using DNVGL.Veracity.Services.Api.Models;
using DNVGL.Veracity.Services.Api.My.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My
{
    public class MyMessages : IMyMessages
    {
        private readonly ApiClientFactory _apiClientFactory;
        public MyMessages(ApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
        }

        /// <summary>
        /// Retrieves a collection of messages addressed to the authenticated user.
        /// </summary>
        /// <param name="includeRead">Set this to true to include messages marked as read.</param>
        /// <returns></returns>
        public Task<IEnumerable<Message>> List(bool includeRead = false) =>
            _apiClientFactory.GetClient().GetResource<IEnumerable<Message>>(MyMessagesUrls.List(includeRead), false);

		/// <summary>
		/// Retrieves an individual message addressed to the authenticated user.
		/// </summary>
		/// <param name="messageId">The unique identifier for the message to be retrieved.</param>
		/// <returns></returns>
		public Task<Message> Get(string messageId) =>
            _apiClientFactory.GetClient().GetResource<Message>(MyMessagesUrls.Message(messageId));

		/// <summary>
		/// Retrieves the numeric value indicating how many messages have not been marked as read by the authenticated user.
		/// </summary>
		/// <returns></returns>
		public Task<int> GetUnreadCount() =>
            _apiClientFactory.GetClient().GetResource<int>(MyMessagesUrls.UnreadCount, false);


		public Task MarkAllMessagesAsRead() =>
            _apiClientFactory.GetClient().PatchResource(MyMessagesUrls.Root);
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
