namespace MHealth.Api.Models
{
    public class GetAllUsersPaginatedRequest
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }
}
