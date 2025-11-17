using System.Security.Claims;
using CleanArch.Core;
using Microsoft.AspNetCore.Http;

namespace CleanArch.API.Services;

/// <summary>
/// Implementation of <see cref="ICurrentUserService"/> that extracts user information
/// from the current HTTP context claims with lazy initialization.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private User? _cachedUser;
    private bool _initialized;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public User? GetCurrentUser()
    {
        if (!_initialized)
        {
            _cachedUser = LoadCurrentUser();
            _initialized = true;
        }

        return _cachedUser;
    }

    /// <inheritdoc />
    public User User
    {
        get
        {
            var user = GetCurrentUser();
            if (user == null || !user.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            return user;
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated
    {
        get
        {
            var user = GetCurrentUser();
            return user?.IsAuthenticated ?? false;
        }
    }

    private User? LoadCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        var claimsPrincipal = httpContext.User;
        if (claimsPrincipal?.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
        {
            return new User { Id = Guid.Empty, IsAuthenticated = false };
        }

        // Extract user ID from claims (supporting both standard and Keycloak claim types)
        var userIdString = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? claimsPrincipal.FindFirst("sub")?.Value
                           ?? null;
        var userId = userIdString != null ? Guid.Parse(userIdString) : Guid.Empty;

        // Extract username from claims
        var username = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value
                       ?? claimsPrincipal.FindFirst("preferred_username")?.Value
                       ?? string.Empty;

        // Extract email from claims
        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value
                    ?? claimsPrincipal.FindFirst("email")?.Value
                    ?? string.Empty;

        // Extract roles from claims
        var roles = claimsPrincipal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return new User
        {
            Id = userId,
            Username = username,
            Email = email,
            Roles = roles,
            IsAuthenticated = true
        };
    }
}
