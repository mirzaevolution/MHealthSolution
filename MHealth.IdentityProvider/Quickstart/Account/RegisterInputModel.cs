using System.ComponentModel.DataAnnotations;

namespace IdentityServerHost.Quickstart.UI
{
    public class RegisterInputModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public int Gender { get; set; }
        public string AddressLine { get; set; }
        [Required]
        public string City { get; set; }
        public string Region { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
