namespace DNVGL.Veracity.Services.Api.Models
{
    public class AdministratorReference:UserReference
    {
        public string AccessLevelUrl { get; set; }

        public string ServiceId { get; set; }
    }
}
