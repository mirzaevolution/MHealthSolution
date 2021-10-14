using MHealth.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MHealth.Abstracts
{
    public interface IUserProfileService
    {
        Task<AppUserDto> GetById(string id);
        Task<AppUserDto> GetByEmail(string email);
        Task<List<AppUserDto>> GetAll(int skip = 0, int take = 100);
        Task<AppUserDto> UpdateUser(AppUserDto dto);
    }
}
