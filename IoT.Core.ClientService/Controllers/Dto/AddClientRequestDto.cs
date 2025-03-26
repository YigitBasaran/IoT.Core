using IoT.Core.AuthService.Model;

namespace IoT.Core.ClientService.Controllers.Dto
{
    public record AddClientRequestDto(string Name, string Email, RoleEnum Role);
}
