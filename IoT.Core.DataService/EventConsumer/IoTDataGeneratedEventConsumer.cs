using IoT.Core.CommonInfrastructure.EventBus.Shared;
using IoT.Core.DataService.Service;
using MassTransit;

namespace IoT.Core.DataService.EventConsumer
{
    public class IoTDataGeneratedEventConsumer : IConsumer<IoTDataGeneratedEvent>
    {
        private readonly ILogger<IoTDataGeneratedEventConsumer> _logger;
        private readonly IDataService _dataService;

        public IoTDataGeneratedEventConsumer(
            ILogger<IoTDataGeneratedEventConsumer> logger,
            IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task Consume(ConsumeContext<IoTDataGeneratedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation($"Received IoTDataGeneratedEvent with {@event.DataList.Count} items.");

            // 🔹 Convert IoTDataGeneratedEvent to AddDataRequestDto
            var addDataRequestDto = EventToDtoConverter.IoT.Core.DataService.Helpers.EventToDtoConverter.ConvertToAddDataRequestDto(@event);

            // 🔹 Call CreateBulkDataAsync
            var createdDataList = await _dataService.CreateBulkDataAsync(addDataRequestDto);

            _logger.LogInformation($"Created {createdDataList.Count} Data records.");
        }
    }
}