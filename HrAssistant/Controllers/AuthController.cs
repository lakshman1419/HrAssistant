using HrAssistant.Models;
using HrAssistant.Services;
using Microsoft.AspNetCore.Mvc;

namespace HrAssistant.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserAuthenticationService _authenticationService;

    public AuthController(IUserAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest? request)
    {
        if (request is null
            || string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { error = "Username and password are required." });
        }

        var user = _authenticationService.Authenticate(request.Username, request.Password);
        if (user is null)
        {
            return Unauthorized(new { error = "Invalid username or password." });
        }

        return Ok(new LoginResponse(user));
    }
}