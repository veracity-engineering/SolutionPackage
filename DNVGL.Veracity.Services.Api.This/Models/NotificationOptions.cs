using DNVGL.Veracity.Services.Api.Models;

namespace DNVGL.Veracity.Services.Api.This.Models
{
    public class NotificationOptions
    {
        public Message Message { get; set; }

        public string[] Recipients { get; set; }

        public bool HighPriority { get; set; }

        public string ChannelId { get; set; }
    }
}
