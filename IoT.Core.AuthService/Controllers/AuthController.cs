using AutoMapper;
using IoT.Core.AuthService.Controllers.Dto;
using IoT.Core.AuthService.Model;
using IoT.Core.AuthService.Model.Exceptions;
using IoT.Core.AuthService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IoT.Core.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[SwaggerTag("Handles authentication, password setup and update operations.")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public AuthController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Operator")]
    [SwaggerOperation(Summary = "Get all users", Description = "Returns all defined users.")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        var usersDto = _mapper.Map<List<UserResponseDto>>(users);
        return Ok(usersDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login with username and password",
        Description = "Returns JWT token if credentials are valid.")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var token = await _authService.LoginAsync(request.Username, request.Password);
        return Ok(new { Token = token });
    }

    [HttpPost("set-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Set initial password",
        Description = "Sets password for first-time users. Does not require old password.")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequestDto request)
    {
        await _authService.SetPasswordAsync(request.Username, request.NewPassword);
        return NoContent();
    }

    [HttpPut("update-password")]
    [Authorize(Roles = "Operator,Client")]
    [SwaggerOperation(Summary = "Update password", Description = "Updates the password. Requires old password.")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestDto request)
    {
        var usernameClaim = User.FindFirst("username")?.Value;
        if (usernameClaim == null || usernameClaim != request.Username)
        {
            return Forbid();
        }

        await _authService.UpdatePassword(request.Username, request.OldPassword, request.NewPassword);
        return NoContent();
    }
}