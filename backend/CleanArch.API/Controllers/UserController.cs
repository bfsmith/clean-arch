using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CleanArch.Core;
using CleanArch.Logging;

namespace CleanArch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public UserController(ILogger<UserController> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
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
        // Use ICurrentUserService to get the current user (lazily populated on first access)
        var currentUser = _currentUserService.User;

        using (_logger.AddContext(new { UserId = currentUser.Id }))
        {
            _logger.Info("User profile accessed", new { name = currentUser.Username, email = currentUser.Email });
        }

        return Ok(new
        {
            message = "This is a protected endpoint. Authentication required.",
            user = new
            {
                userId = currentUser.Id,
                username = currentUser.Username,
                email = currentUser.Email,
                roles = currentUser.Roles,
                isAuthenticated = currentUser.IsAuthenticated
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

