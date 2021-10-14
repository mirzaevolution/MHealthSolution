using AutoMapper;
using MHealth.Api.Models;
using MHealth.BusinessEntities;
using MHealth.DataTransferObjects;

namespace MHealth.Api
{
    public class AutomapperConfiguration:Profile
    {
        public AutomapperConfiguration()
        {
            CreateMap<AppUser, AppUserDto>().ReverseMap();
            CreateMap<AppUserGender, AppUserGenderDto>().ReverseMap();
            CreateMap<AppUserDto, BaseUserResponse>().ReverseMap();
            CreateMap<UpdateUserRequest, AppUserDto>();
        }
    }
}
