using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    /// <summary>
    /// Public endpoint - no authentication required
    /// </summary>
    [HttpGet("public")]
    public IActionResult GetPublicInfo()
    {
        return Ok(new
        {
            message = "This is a public endpoint. No authentication required.",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Protected endpoint - requires JWT authentication
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("sub")?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value 
                       ?? User.FindFirst("preferred_username")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value 
                    ?? User.FindFirst("email")?.Value;

        return Ok(new
        {
            message = "This is a protected endpoint. Authentication required.",
            user = new
            {
                userId,
                username,
                email,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            },
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Protected endpoint - requires authentication and specific role
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "admin")]
    public IActionResult GetAdminInfo()
    {
        return Ok(new
        {
            message = "This endpoint requires admin role.",
            user = User.Identity?.Name,
            timestamp = DateTime.UtcNow
        });
    }
}

