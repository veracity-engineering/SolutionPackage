namespace DNVGL.Veracity.Services.Api.Models
{
    public class Administrator : User
    {
        public RoleReference[] Roles { get; set; }
    }
}
