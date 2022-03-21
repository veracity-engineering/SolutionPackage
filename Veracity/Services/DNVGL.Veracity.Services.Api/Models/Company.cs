using Newtonsoft.Json;
using System.Collections.Generic;

namespace DNVGL.Veracity.Services.Api.Models
{
    public class Company
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string UsersUrl { get; set; }
        public IEnumerable<string> AddressLines { get; set; }
        public string Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }

        [JsonProperty("#employees")]
        public uint NumberOfEmployees { get; set; }
        public string Email { get; set; }

        [JsonProperty("#requests")]
        public uint NumberOfRequests { get; set; }
        public string InternalId { get; set; }
    }
}
