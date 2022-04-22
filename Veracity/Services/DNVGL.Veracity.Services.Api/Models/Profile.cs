using System.Text.Json.Serialization;

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

        [JsonPropertyName("#companies")]
        public uint NumberOfCompanies { get; set; }

        [JsonPropertyName("verifiedEmail")]
        public bool IsEmailVerified { get; set; }
        public string Language { get; set; }
        public string Phone { get; set; }

        [JsonPropertyName("verifiedPhone")]
        public bool IsPhoneVerified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }

        [JsonPropertyName("managedAccount")]
        public bool IsAccountManaged { get; set; }

        [JsonPropertyName("activated")]
        public bool IsActivated { get; set; }
    }
}
