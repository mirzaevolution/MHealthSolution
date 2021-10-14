using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MHealth.SharedDataAccess;
using MHealth.DataTransferObjects;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MHealth.BusinessEntities;
using AutoMapper;
using MHealth.Abstracts;

namespace MHealth.Services
{

   

    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UserProfileService(
                ApplicationDbContext context,
                IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<AppUserDto> GetById(string id)
        {
            AppUser entity= await _context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            var dto = _mapper.Map<AppUser, AppUserDto>(entity);
            return dto;
        }
        public async Task<AppUserDto> GetByEmail(string email)
        {
            AppUser entity = await _context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);
            var dto = _mapper.Map<AppUser, AppUserDto>(entity);
            return dto;
        }
        public async Task<List<AppUserDto>> GetAll(int skip=0, int take= 100)
        {
            List<AppUser> entities = await _context
                .Users
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
            var dtoList = _mapper.Map<List<AppUser>, List<AppUserDto>>(entities);
            return dtoList;
        }

        public async Task<AppUserDto> UpdateUser(AppUserDto dto)
        {
            var entity = await _context
                .Users
                .FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (entity == null)
                throw new Exception($"User with id: {dto.Id} is not found");
            entity.FullName = dto.FullName;
            entity.PhotoUrl = dto.PhotoUrl;
            entity.Gender = (AppUserGender)dto.Gender;
            entity.AddressLine = dto.AddressLine;
            entity.City = dto.City;
            entity.Region = dto.Region;
            entity.Country = dto.Country;
            entity.PhoneNumber = dto.PhoneNumber;
            _context.Users.Attach(entity);

            var entry = _context.Entry<AppUser>(entity);
            entry.State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            return result > 0 ? dto : null;
        }

    }
}
