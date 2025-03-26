using Bogus;
using IoT.Core.CommonInfrastructure.EventBus.Shared;

namespace IoT.Core.MockGroundService.Controllers;

public class IoTDataGenerator
{
    private readonly List<string> _devEuis;
    private const int DataCountPerDevEui = 10;

    public IoTDataGenerator()
    {
        _devEuis = new List<string> { "0000000000000001", "0000000000000002", "0000000000000003" };
    }

    public List<IoTData> GenerateIoTData()
    {
        var faker = new Faker();
        var dataList = new List<IoTData>();

        foreach (var devEui in _devEuis)
        {
            for (int i = 0; i < DataCountPerDevEui; i++)
            {
                var timestamp = DateTime.SpecifyKind(
                    faker.Date.Between(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow),
                    DateTimeKind.Utc);

                var payload = new
                {
                    Temperature = faker.Random.Double(10, 40),
                    Humidity = faker.Random.Double(30, 80),
                    GPS = new { Lat = faker.Address.Latitude(), Long = faker.Address.Longitude() },
                    Timestamp = timestamp
                };

                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                dataList.Add(new IoTData(devEui, jsonPayload));
            }
        }

        return dataList;
    }
}