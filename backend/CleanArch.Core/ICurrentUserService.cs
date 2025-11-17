namespace CleanArch.Core;

/// <summary>
/// Service for accessing the current authenticated user information.
/// The user information is lazily populated on first access and cached for the request lifetime.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user, or null if the user is not authenticated.
    /// </summary>
    /// <returns>The current user, or null if not authenticated.</returns>
    User? GetCurrentUser();

    /// <summary>
    /// Gets the current authenticated user.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the user is not authenticated.</exception>
    User User { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}

