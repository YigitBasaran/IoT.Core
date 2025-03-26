namespace IoT.Core.AuthService.Controllers.Dto
{
    public record UpdatePasswordRequestDto(string Username, string OldPassword, string NewPassword);

}
