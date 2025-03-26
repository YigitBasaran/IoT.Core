using IoT.Core.CommonInfrastructure.EventBus.Publisher;
using IoT.Core.CommonInfrastructure.EventBus.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IoT.Core.MockGroundService.Controllers
{
    [ApiController]
    [Route("api/ground")]
    public class GroundController : ControllerBase
    {
        private readonly IoTDataGenerator _dataGenerator;
        private readonly IEventPublisher _publisher;
        private readonly ILogger<GroundController> _logger;

        public GroundController(
            IoTDataGenerator dataGenerator,
            IEventPublisher publisher,
            ILogger<GroundController> logger)
        {
            _dataGenerator = dataGenerator;
            _publisher = publisher;
            _logger = logger;
        }

        [HttpPost("publish")]
        public async Task<IActionResult> PublishData(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating IoT Data...");
            var iotData = _dataGenerator.GenerateIoTData();

            var iotDataEvent = new IoTDataGeneratedEvent(iotData);

            _logger.LogInformation("Publishing IoT Data to RabbitMQ...");
            await _publisher.PublishAsync("iot_data_exchange", "iot.data", iotDataEvent, cancellationToken);

            return Ok(new { message = "IoT Data Published Successfully", count = iotData.Count });
        }
    }
}