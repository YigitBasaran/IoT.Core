using IoT.Core.DataService.Model;
using IoT.Core.DataService.Service;
using IoT.Core.DataService.Controllers.Dto;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IoT.Core.DataService.Controllers
{
    [ApiController]
    [Route("api/data")]
    [Produces("application/json")]
    [SwaggerTag("Handles IoT data operations including retrieval, bulk insertion, and bulk deletion.")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IMapper _mapper;

        public DataController(IDataService dataService, IMapper mapper)
        {
            _dataService = dataService;
            _mapper = mapper;
        }

        [HttpGet("device-id/{devEui}")]
        [Authorize(Roles = "Operator,Client")]
        [SwaggerOperation(Summary = "Retrieve IoT data by DevEUI",
                          Description = "Returns a list of IoT data associated with the given DevEUI within the specified time range.")]
        public async Task<ActionResult<List<DataResponseDto>>> GetDataByDevEui(
            string devEui, [FromQuery] DateTime startDateTime, [FromQuery] DateTime endDateTime)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var clientIdClaim = User.FindFirst("clientId")?.Value;


            if (startDateTime >= endDateTime)
            {
                return BadRequest("Start date must be earlier than end date.");
            }
            var dataList = await _dataService.GetDataByDevEuiBetweenTimespanAsync(devEui, startDateTime, endDateTime);

            // If Role is Client and id in path and clientId in claims are not match --> forbidden
            if (userRole == "Client" && clientIdClaim != dataList.ElementAt(0).ClientId.ToString())
            {
                return Forbid();
            }

            var dataDto = _mapper.Map<List<DataResponseDto>>(dataList);
            return Ok(dataDto);
        }

        [HttpGet("client-id/{clientId}")]
        [Authorize(Roles = "Operator,Client")]
        [SwaggerOperation(Summary = "Retrieve IoT data by Client ID",
                          Description = "Returns all IoT data associated with the specified client within the given time range.")]
        public async Task<ActionResult<List<DataResponseDto>>> GetDataByClientId(
            int clientId, [FromQuery] DateTime startDateTime, [FromQuery] DateTime endDateTime)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var clientIdClaim = User.FindFirst("clientId")?.Value;

            // If Role is Client and id in path and clientId in claims are not match --> forbidden
            if (userRole == "Client" && clientIdClaim != clientId.ToString())
            {
                return Forbid();
            }
            if (startDateTime >= endDateTime)
            {
                return BadRequest("Start date must be earlier than end date.");
            }
            var startUtc = DateTime.SpecifyKind(startDateTime, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(endDateTime, DateTimeKind.Utc);
            var dataList = await _dataService.GetDataByClientIdBetweenTimespanAsync(clientId, startUtc, endUtc);
            var dataDto = _mapper.Map<List<DataResponseDto>>(dataList);
            return Ok(dataDto);
        }

        [HttpDelete("device-id/{devEui}")]
        [Authorize(Roles = "Operator")]
        [SwaggerOperation(Summary = "Delete IoT data by DevEUI",
                          Description = "Removes all IoT data associated with the specified DevEUI.")]
        public async Task<IActionResult> DeleteDataByDevEui(string devEui)
        {
            await _dataService.DeleteDataByDevEuiAsync(devEui);
            return NoContent();
        }

        [HttpDelete("client-id/{clientId}")]
        [Authorize(Roles = "Operator")]
        [SwaggerOperation(Summary = "Delete IoT data by ClientId",
            Description = "Removes all IoT data associated with the specified ClientId.")]
        public async Task<IActionResult> DeleteDataByClientId(int clientId)
        {
            await _dataService.DeleteDataByClientIdAsync(clientId);
            return NoContent();
        }
    }
}
