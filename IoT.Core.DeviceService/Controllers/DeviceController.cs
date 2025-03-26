using AutoMapper;
using IoT.Core.DeviceService.Service;
using Microsoft.AspNetCore.Mvc;
using IoT.Core.DeviceService.Controllers.Dto;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using IoT.Core.DeviceService.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IoT.Core.DeviceService.Controllers;

[ApiController]
[Route("api/device")]
[Produces("application/json")]
[SwaggerTag("Manages IoT devices, including creation, retrieval, updating, and deletion.")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly IValidator<AddDeviceRequestDto> _addValidator;
    private readonly IValidator<UpdateDeviceNameRequestDto> _updateNameValidator;
    private readonly IMapper _mapper;

    public DeviceController(IDeviceService deviceService,
        IValidator<AddDeviceRequestDto> addValidator,
        IValidator<UpdateDeviceNameRequestDto> updateNameValidator, IMapper mapper)
    {
        _deviceService = deviceService;
        _addValidator = addValidator;
        _updateNameValidator = updateNameValidator;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Retrieve all IoT devices",
        Description = "Returns a list of all registered IoT devices.")]
    public async Task<ActionResult<IEnumerable<DeviceResponseDto>>> GetDevices()
    {
        var devices = await _deviceService.GetDevicesAsync();
        var deviceDtos = _mapper.Map<List<DeviceResponseDto>>(devices);
        return Ok(deviceDtos);
    }

    [HttpGet("client-id/{clientId}")]
    [Authorize(Roles = "Operator,Client")]
    [SwaggerOperation(Summary = "Retrieve devices by client ID",
        Description = "Returns all devices associated with the specified client ID.")]
    public async Task<ActionResult<IEnumerable<DeviceResponseDto>>> GetDevicesByClientId(int clientId)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var clientIdClaim = User.FindFirst("clientId")?.Value;

        // If Role is Client and id in path and clientId in claims are not match --> forbidden
        if (userRole == "Client" && clientIdClaim != clientId.ToString())
        {
            return Forbid();
        }

        var devices = await _deviceService.GetDevicesByClientIdAsync(clientId);
        var deviceDtos = _mapper.Map<List<DeviceResponseDto>>(devices);
        return Ok(deviceDtos);
    }

    [HttpGet("{devEui}")]
    [Authorize(Roles = "Operator,Client")]
    [SwaggerOperation(Summary = "Retrieve a device by ID",
        Description = "Returns details of a specific IoT device using its unique identifier.")]
    public async Task<ActionResult<DeviceResponseDto>> GetDevice(string devEui)
    {
        var device = await _deviceService.GetDeviceByIdAsync(devEui);
        if (device == null)
        {
            return NotFound();
        }
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var clientIdClaim = User.FindFirst("clientId")?.Value;

        // If Role is Client and id in path and clientId in claims are not match --> forbidden
        if (userRole == "Client" && clientIdClaim != device.ClientId.ToString())
        {
            return Forbid();
        }
        var deviceDto = _mapper.Map<DeviceResponseDto>(device);
        return Ok(deviceDto);
    }

    [HttpPost]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Create a new IoT device",
        Description = "Registers a new IoT device in the system.")]
    public async Task<ActionResult<DeviceResponseDto>> CreateDevice([FromBody] AddDeviceRequestDto request)
    {
        var validationResult = await _addValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdDevice = await _deviceService.CreateDeviceAsync(request);
        var createdDeviceDto = _mapper.Map<DeviceResponseDto>(createdDevice);
        return CreatedAtAction(nameof(GetDevice), new { devEui = createdDeviceDto.DevEui }, createdDeviceDto);
    }

    [HttpPut("name")]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Update name of an existing device",
        Description = "Updates name of an existing IoT device.")]
    public async Task<IActionResult> UpdateDeviceName([FromBody] UpdateDeviceNameRequestDto nameRequest)
    {
        var validationResult = await _updateNameValidator.ValidateAsync(nameRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await _deviceService.UpdateDeviceNameAsync(nameRequest);
        return NoContent();
    }

    [HttpPut("location")]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Update location of an existing device",
        Description = "Updates location of an existing IoT device.")]
    public async Task<IActionResult> UpdateDeviceLocation([FromBody] UpdateDeviceLocationRequestDto locationRequest)
    {
        await _deviceService.UpdateDeviceLocationAsync(locationRequest);
        return NoContent();
    }

    [HttpDelete("{devEui}")]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Delete an IoT device",
        Description = "Removes an IoT device from the system using its unique identifier.")]
    public async Task<IActionResult> DeleteDevice(string devEui)
    {
        try
        {
            var jwt = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..].Trim();
            await _deviceService.DeleteDeviceAsync(devEui, jwt);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpDelete("delete/client-id/{clientId}")]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Delete an IoT device",
        Description = "Removes an IoT device from the system using its unique identifier.")]
    public async Task<IActionResult> DeleteDevicesByClientId(int clientId)
    {
        try
        {
            var jwt = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..].Trim();
            await _deviceService.DeleteDevicesByClientId(clientId, jwt);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
}