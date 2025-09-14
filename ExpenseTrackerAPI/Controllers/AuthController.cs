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
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        try
        {
            var token = await authService.Login(loginDTO);
            return Ok(new { token });
        }
        catch (LoginException)
        {
            return Unauthorized(new { message = "Incorrect email or password"});
        }
    }

    [HttpPost, Route("register")]
    public async Task<IActionResult> Register(RegisterDTO registerDto)
    {
        var token = await authService.Register(registerDto);
        return Ok(new { token });
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