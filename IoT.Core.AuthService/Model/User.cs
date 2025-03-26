using Consul;
using IoT.Core.CommonInfrastructure.Database.Repo;

namespace IoT.Core.AuthService.Model
{
    public class User : BaseEntity<int>
    {
        public string Username { get; set; }
        public RoleEnum Role { get; set; }
        public string PasswordHash { get; set; }

        public User(string username, string passwordHash, RoleEnum role)
        {
            this.Username = username;
            this.PasswordHash = passwordHash;
            this.Role = role;
        }

        public static User OnCreate(int id,string username, RoleEnum role, string passwordHash = "")
        {
            var user = new User(username, passwordHash, role);
            user.Id = id;
            user.CreatedAt = DateTime.UtcNow;
            return user;
        }

        public void OnUpdatePassword(string newPassword)
        {
            this.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void OnSetPassword(string password)
        {
            this.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}
