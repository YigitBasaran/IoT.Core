using IoT.Core.CommonInfrastructure.Database.Repo;
using System.Text.Json.Nodes;

namespace IoT.Core.DataService.Model
{
    public class Data : BaseEntity<Guid>
    {
        public int ClientId { get; set; }
        public string DevEui { get; set; }
        public string Payload { get; set; }
        public Data(Guid id, int clientId, string devEui, string payload)
        {
            this.Id = id;
            this.ClientId = clientId;
            this.DevEui = devEui;
            this.Payload = payload;
        }

        public static Data OnCreate(int clientId, string devEui, string payload)
        {
            var jsonPayload = JsonNode.Parse(payload);

            DateTime createdAt = jsonPayload?["Timestamp"]?.GetValue<DateTime>() ?? DateTime.UtcNow;

            createdAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);

            jsonPayload?.AsObject().Remove("Timestamp");

            string cleanedPayload = jsonPayload?.ToJsonString() ?? payload;

            var data = new Data(Guid.NewGuid(), clientId, devEui, cleanedPayload);

            data.CreatedAt = createdAt;

            return data;
        }
    }
}
