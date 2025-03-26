namespace IoT.Core.DataService.EventConsumer.EventToDtoConverter
{
    using global::IoT.Core.CommonInfrastructure.EventBus.Shared;
    using global::IoT.Core.DataService.Controllers.Dto;

    namespace IoT.Core.DataService.Helpers
    {
        public static class EventToDtoConverter
        {
            public static AddDataRequestDto ConvertToAddDataRequestDto(IoTDataGeneratedEvent @event)
            {
                var dataRequestDtoList = @event.DataList
                    .Select(data => new DataRequestDto(data.DevEui, data.Payload))
                    .ToList();

                return new AddDataRequestDto(dataRequestDtoList);
            }
        }
    }
}
