using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CleanArch.Logging;

namespace CleanArch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Public endpoint - no authentication required
    /// </summary>
    [HttpGet("public")]
    public IActionResult GetPublicInfo()
    {
        _logger.Info("Public endpoint accessed");
        
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

        using (_logger.AddContext(new { UserId = userId }))
        {
            _logger.Info("User profile accessed", new { name = username, email });
        }

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
        _logger.Debug("Admin endpoint accessed", new { user = User.Identity?.Name });
        _logger.Warn("Admin access granted", new { role = "admin" });
        
        return Ok(new
        {
            message = "This endpoint requires admin role.",
            user = User.Identity?.Name,
            timestamp = DateTime.UtcNow
        });
    }
}

