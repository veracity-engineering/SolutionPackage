using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using DNVGL.Veracity.Services.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.ApiV3
{
    public class MyMessages : ApiResourceClient, IMyMessages
    {
        public MyMessages(IOAuthHttpClientFactory httpClientFactory, ISerializer serializer, string clientConfigurationName) : base(httpClientFactory, serializer, clientConfigurationName)
        {
        }

        public async Task<IEnumerable<Message>> List(bool includeRead = false)
        {
            var response = await GetOrCreateHttpClient().GetAsync(MyMessagesUrls.List(includeRead));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<IEnumerable<Message>>(content);
        }

        public async Task<Message> Get(string messageId)
        {
            var response = await GetOrCreateHttpClient().GetAsync(MyMessagesUrls.Message(messageId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<Message>(content);
        }

        public async Task<int> GetUnreadCount()
        {
            var response = await GetOrCreateHttpClient().GetAsync(MyMessagesUrls.UnreadCount);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Deserialize<int>(content);
        }
    }

    internal class MyMessagesUrls
    {
        public static string Root => "/veracity/services/v3/my/messages";

        public static string List(bool includeRead) => includeRead
            ? $"{Root}?all=true"
            : Root;

        public static string Message(string messageId) => $"{Root}/{messageId}";

        public static string UnreadCount => $"{Root}/count";
    }
}
