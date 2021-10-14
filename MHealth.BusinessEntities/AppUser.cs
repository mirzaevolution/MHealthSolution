using Microsoft.AspNetCore.Identity;

namespace MHealth.BusinessEntities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public AppUserGender Gender { get; set; }
        public string PhotoUrl { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }
}
