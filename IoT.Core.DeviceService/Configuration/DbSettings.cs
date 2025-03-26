using IoT.Core.CommonInfrastructure.Extensions.DbSettings;

namespace IoT.Core.DeviceService.Configuration
{
    public record DbSettings(string ConnectionString, string DatabaseName, string CollectionName)
        : MongoDbSettings(ConnectionString, DatabaseName, CollectionName)
    {
        public static DbSettings MapFromMongoDbSettingsToDbSettings(MongoDbSettings mongoDbSettings)
        {
            return new DbSettings(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName,
                mongoDbSettings.CollectionName);
        }
    }

}
