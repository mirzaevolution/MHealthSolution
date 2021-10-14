using System.Collections.Generic;

namespace MHealth.Api.Models
{
    public class GetAllUsersPaginatedResponse
    {
        public List<BaseUserResponse> Users { get; set; }
    }
}
