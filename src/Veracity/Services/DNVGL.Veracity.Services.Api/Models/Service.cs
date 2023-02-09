using System;
using System.Text.Json.Serialization;

namespace DNVGL.Veracity.Services.Api.Models
{
    public class Service
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string ApiAudience { get; set; }

        public string Category { get; set; }

        [JsonPropertyName("public")]
        public bool IsPublic { get; set; }

        [JsonPropertyName("inherited")]
        public bool IsInherited { get; set; }

        [JsonPropertyName("selfSubscribe")]
        public bool IsSelfSubscribable { get; set; }

        public string ServiceOwner { get; set; }

        public string TermsOfUser { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string ParentUrl { get; set; }

        public string ParentId { get; set; }

        public string ChildrenUrl { get; set; }

        public string ServiceUrl { get; set; }
    }
}
