using EmployeeManagementAPI.Models.Auth;
using EmployeeManagementAPI.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(LoginRequest request)
    {
        try
        {
            await _authService.RegisterAsync(
                request.Username,
                request.Password);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "User registered successfully."
                });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(
                request.Username,
                request.Password);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }
}