namespace IoT.Core.DeviceService.Configuration;

public record DbSettings(string ConnectionString, string DatabaseName, string CollectionName);