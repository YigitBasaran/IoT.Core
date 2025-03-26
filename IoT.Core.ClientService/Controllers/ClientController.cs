using System.Security.Claims;
using AutoMapper;
using IoT.Core.ClientService.Service;
using IoT.Core.ClientService.Model;
using IoT.Core.ClientService.Controllers.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IoT.Core.ClientService.Controllers;

[ApiController]
[Route("api/client")]
[Produces("application/json")]
[SwaggerTag("Manages clients, including creation, retrieval, updating, and deletion.")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IValidator<AddClientRequestDto> _addValidator;
    private readonly IValidator<UpdateClientEmailRequestDto> _updateEmailValidator;
    private readonly IMapper _mapper;

    public ClientController(
        IClientService clientService,
        IValidator<AddClientRequestDto> addValidator,
        IValidator<UpdateClientEmailRequestDto> updateEmailValidator, IMapper mapper)
    {
        _clientService = clientService;
        _addValidator = addValidator;
        _updateEmailValidator = updateEmailValidator;
        _mapper = mapper;
    }


    [HttpGet]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Retrieve all clients",
        Description = "Returns a list of all registered clients.")]
    public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetClients()
    {
        var clients = await _clientService.GetAllClientsAsync();
        var clientDtos = _mapper.Map<List<ClientResponseDto>>(clients);

        return Ok(clientDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Operator,Client")]
    [SwaggerOperation(Summary = "Retrieve a client by ID",
        Description = "Returns details of a specific client using its unique identifier.")]
    public async Task<ActionResult<ClientResponseDto>> GetClient(int id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var clientIdClaim = User.FindFirst("clientId")?.Value;

        // If Role is Client and id in path and clientId in claims are not match --> forbidden
        if (userRole == "Client" && clientIdClaim != id.ToString())
        {
            return Forbid();
        }

        var client = await _clientService.GetClientByIdAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        var clientDto = _mapper.Map<ClientResponseDto>(client);
        return Ok(clientDto);
    }

    [HttpPost]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Create a new client",
        Description = "Registers a new client in the system.")]
    public async Task<ActionResult<ClientResponseDto>> CreateClient([FromBody] AddClientRequestDto request)
    {
        var validationResult = await _addValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var client = await _clientService.CreateClientAsync(request.Name, request.Role ,request.Email);
        var clientDto = _mapper.Map<ClientResponseDto>(client);

        return CreatedAtAction(nameof(GetClient), new { id = clientDto.Id }, clientDto);
    }

    [HttpPut("email")]
    [Authorize(Roles = "Client")]
    [SwaggerOperation(Summary = "Update client email",
        Description = "Updates the email of an existing client.")]
    public async Task<IActionResult> UpdateClientEmail([FromBody] UpdateClientEmailRequestDto emailRequest)
    {
        var clientIdClaim = User.FindFirst("clientId")?.Value;

        // If id in request dto and clientId in claims are not match --> forbidden
        if (clientIdClaim != emailRequest.Id.ToString())
        {
            return Forbid();
        }

        var validationResult = await _updateEmailValidator.ValidateAsync(emailRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await _clientService.UpdateClientEmailAsync(emailRequest.Id, emailRequest.Email);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Delete a client",
        Description = "Removes a client from the system using its unique identifier.")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            var jwt = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..].Trim();
            await _clientService.DeleteClientAsync(id, jwt);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
