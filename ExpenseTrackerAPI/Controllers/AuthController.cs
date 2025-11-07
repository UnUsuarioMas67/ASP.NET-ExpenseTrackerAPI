using System.Security.Claims;
using System.Security.Cryptography;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ExpenseTrackerAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost, Route("login")]
    public async Task<IActionResult> Login(LoginRequest login)
    {
        var result = await authService.Login(login);
        if (result.IsSuccess)
            return Ok(new {token = result.Value});
        
        return Ok(new {error = result.Error});
    }

    [HttpPost, Route("register")]
    public async Task<IActionResult> Register(RegisterRequest register)
    {
        var result = await authService.Register(register);
        if (result.IsSuccess)
            return Ok(new {token = result.Value});
        
        return Ok(new {error = result.Error});
    }

    [HttpGet, Route("test")]
    [Authorize]
    public IActionResult Test()
    {
        var username = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var id = HttpContext.User.FindFirst("id")?.Value;

        return Ok(new { username, email, id });
    }
}