using IoT.Core.DeviceService.Service;
using Microsoft.AspNetCore.Mvc;
using IoT.Core.DeviceService.Controllers.Dto;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace IoT.Core.DeviceService.Controllers;

[ApiController]
[Route("api/device")]
[Produces("application/json")]
[SwaggerTag("Manages IoT devices, including creation, retrieval, updating, and deletion.")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly IValidator<AddDeviceRequestDto> _addValidator;
    private readonly IValidator<UpdateDeviceRequestDto> _updateValidator;

    public DeviceController(IDeviceService deviceService,
        IValidator<AddDeviceRequestDto> addValidator,
        IValidator<UpdateDeviceRequestDto> updateValidator)
    {
        _deviceService = deviceService;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Retrieve all IoT devices",
        Description = "Returns a list of all registered IoT devices.")]
    public async Task<ActionResult<IEnumerable<Model.Device>>> GetDevices()
    {
        var devices = await _deviceService.GetDevicesAsync();
        return Ok(devices);
    }

    [HttpGet("customer-id/{customerId}")]
    [SwaggerOperation(Summary = "Retrieve devices by customer ID",
        Description = "Returns all devices associated with the specified customer ID.")]
    public async Task<ActionResult<IEnumerable<Model.Device>>> GetDevicesByCustomerId(int customerId)
    {
        var devices = await _deviceService.GetDevicesByCustomerId(customerId);
        return Ok(devices);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve a device by ID",
        Description = "Returns details of a specific IoT device using its unique identifier.")]
    public async Task<ActionResult<Model.Device>> GetDevice(Guid id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        if (device == null)
        {
            return NotFound();
        }

        return Ok(device);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new IoT device",
        Description = "Registers a new IoT device in the system.")]
    public async Task<ActionResult<Model.Device>> CreateDevice([FromBody] AddDeviceRequestDto request)
    {
        var validationResult = await _addValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdDevice = await _deviceService.CreateDeviceAsync(request);
        return CreatedAtAction(nameof(GetDevice), new { id = createdDevice.Id }, createdDevice);
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Update an existing device",
        Description = "Updates details of an existing IoT device.")]
    public async Task<IActionResult> UpdateDevice([FromBody] UpdateDeviceRequestDto request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var existingDevice = await _deviceService.GetDeviceByIdAsync(request.Id);
        if (existingDevice == null)
        {
            return NotFound();
        }

        await _deviceService.UpdateDeviceAsync(request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete an IoT device",
        Description = "Removes an IoT device from the system using its unique identifier.")]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        try
        {
            await _deviceService.DeleteDeviceAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}