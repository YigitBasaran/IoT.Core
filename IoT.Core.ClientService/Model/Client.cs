using IoT.Core.CommonInfrastructure.Database.Repo;

namespace IoT.Core.ClientService.Model
{
    public class Client : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public Client(string name, string email)
        {
            this.Name = name;
            this.Email = email;
        }

        public static Client OnCreate(string name, string email)
        {
            var client = new Client(name, email);
            client.CreatedAt = DateTime.UtcNow;
            return client;
        }

        public void OnUpdateEmail(string email)
        {
            this.Email = email;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}
