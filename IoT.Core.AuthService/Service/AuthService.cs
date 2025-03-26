
using IoT.Core.AuthService.Model;
using IoT.Core.AuthService.Model.Exceptions;
using IoT.Core.AuthService.Repo;
using IoT.Core.AuthService.Service.Jwt;
using IoT.Core.CommonInfrastructure.Auth;

namespace IoT.Core.AuthService.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepo _userRepo;

        public AuthService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _userRepo.GetAllAsync()).ToList();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepo.FindByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(int id, string username, RoleEnum role)
        {
            var user = User.OnCreate(id, username, role);

            await _userRepo.AddAsync(user);
            return user;
        }

        public async Task SetPasswordAsync(string username, string newPassword)
        {
            var user = (await _userRepo.FindAsync(user => user.Username == username)).FirstOrDefault();
            if (user == null) throw new PasswordSetInvalidException();
            if (!string.IsNullOrEmpty(user.PasswordHash)) throw new PasswordSetInvalidException();

            user.OnSetPassword(newPassword);
            await _userRepo.UpdateAsync(user);
        }

        public async Task UpdatePassword(string username, string oldPassword, string newPassword)
        {
            var user = (await _userRepo.FindAsync(user => user.Username == username)).FirstOrDefault();
            if (user == null) throw new PasswordUpdateInvalidException();

            if (string.IsNullOrEmpty(newPassword))
                throw new PasswordUpdateInvalidException();

            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new PasswordUpdateInvalidException();

            if(!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                throw new PasswordUpdateInvalidException();

            user.OnUpdatePassword(newPassword);

            await _userRepo.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepo.FindByIdAsync(id);
            if (user == null) throw new UserNotFoundException();

            await _userRepo.RemoveAsync(user);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = (await _userRepo.FindAsync(u => u.Username == username)).FirstOrDefault();

            if (user == null) throw new LoginException();
            if (string.IsNullOrEmpty(user.PasswordHash)) throw new LoginException();
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) throw new LoginException();

            var token = JwtTokenHelper.GenerateToken(JwtSettings.Secret, user.Id, user.Role.ToString(), user.Username);
            return token;
        }
    }
}
