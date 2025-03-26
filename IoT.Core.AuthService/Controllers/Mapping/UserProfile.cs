using AutoMapper;
using IoT.Core.AuthService.Controllers.Dto;
using IoT.Core.AuthService.Model;

namespace IoT.Core.AuthService.Controllers.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponseDto>()
                .ConvertUsing(src => new UserResponseDto(
                    src.Id,
                    src.Username,
                    src.Role.ToString()));
        }
    }
}