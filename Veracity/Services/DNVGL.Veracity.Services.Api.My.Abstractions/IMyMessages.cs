using DNVGL.Veracity.Services.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
    public interface IMyMessages
    {
		/// <summary>
		/// Returns a collection of messages for the authenticated user, filtered by unread unless specified.
		/// </summary>
		/// <param name="includeRead">Set to true to include all messages, read and unread.</param>
		/// <returns></returns>
		Task<IEnumerable<Message>> List(bool includeRead = false);

		/// <summary>
		/// Returns a message received by authenticated user by message id.
		/// </summary>
		/// <param name="messageId"></param>
		/// <returns></returns>
		[System.Obsolete("This endpoint is hidden")]
		Task<Message> Get(string messageId);

		/// <summary>
		/// Returns the number of unread messages for the authenticated user.
		/// </summary>
		/// <returns></returns>
		Task<int> GetUnreadCount();

		/// <summary>
		/// Marks all unread messages as read
		/// </summary>
		/// <returns></returns>
		Task MarkAllMessagesAsRead();

	}
}
