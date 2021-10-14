namespace MHealth.Api.Models
{
    public class BaseUserResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public UserGender Gender { get; set; }

        public string AddressLine { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
    }
}
