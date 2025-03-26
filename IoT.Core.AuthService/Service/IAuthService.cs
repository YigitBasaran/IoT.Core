using IoT.Core.AuthService.Model;

namespace IoT.Core.AuthService.Service
{
    public interface IAuthService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(int id, string username, RoleEnum role);
        Task SetPasswordAsync(string username, string newPassword);
        Task UpdatePassword(string username, string oldPassword, string newPassword);
        Task DeleteUserAsync(int id);
        Task<string> LoginAsync(string username, string password);
    }
}
