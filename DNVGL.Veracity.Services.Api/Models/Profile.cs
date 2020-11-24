using Newtonsoft.Json;

namespace DNVGL.Veracity.Services.Api.Models
{
    public class Profile
    {
        public string ProfilePageUrl { get; set; }
        public string MessageUrl { get; set; }
        public string Identity { get; set; }
        public string ServicesUrl { get; set; }
        public string CompaniesUrl { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

        [JsonProperty("#companies")]
        public uint NumberOfCompanies { get; set; }

        [JsonProperty("verifiedEmail")]
        public bool IsEmailVerified { get; set; }
        public string Language { get; set; }
        public string Phone { get; set; }

        [JsonProperty("verifiedPhone")]
        public bool IsPhoneVerified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }

        [JsonProperty("managedAccount")]
        public bool IsAccountManaged { get; set; }

        [JsonProperty("activated")]
        public bool IsActivated { get; set; }
    }
}
