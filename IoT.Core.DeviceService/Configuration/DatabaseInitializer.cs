using IoT.Core.DeviceService.Model;
using MongoDB.Driver;

namespace IoT.Core.DeviceService.Configuration
{
    public class DatabaseInitializer
    {
        private readonly IMongoCollection<Device> _devicesCollection;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IMongoDatabase database, ILogger<DatabaseInitializer> logger)
        {
            _devicesCollection = database.GetCollection<Device>("Devices");
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            var existingDevices = await _devicesCollection.CountDocumentsAsync(_ => true);
            if (existingDevices > 0)
            {
                _logger.LogInformation("Mock data already exists. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Seeding mock devices...");

            var devices = new List<Device>
            {
                new Device("0000000000000001", "Sensor A", 1, new Location("USA", "California", "Los Angeles")),
                new Device("0000000000000002", "Sensor B", 2, new Location("UK", "London", "Westminster")),
                new Device("0000000000000003", "Sensor C", 3, new Location("Germany", "Berlin", "Mitte"))
            };

            await _devicesCollection.InsertManyAsync(devices);
            _logger.LogInformation("Mock devices inserted successfully.");
        }
    }
}