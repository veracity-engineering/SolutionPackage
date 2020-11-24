namespace DNVGL.Veracity.Services.Api.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public uint NumberOfCompanies { get; set; }
        public bool VerifiedEmail { get; set; }
        public string Language { get; set; }
        public string Identity { get; set; }
        public string Phone { get; set; }
        public bool VerifiedPhone { get; set; }
        public string ServicesUrl { get; set; }
        public string CompaniesUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }
        public bool ManagedAccount { get; set; }
        public bool Activated { get; set; }
    }
}
