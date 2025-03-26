using IoT.Core.AuthService.Model;
using IoT.Core.CommonInfrastructure.Database.Repo;

namespace IoT.Core.AuthService.Controllers.Dto
{
    public record UserResponseDto(int ClientId, string Username, string Role);
}
