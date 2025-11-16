using Microsoft.Extensions.Configuration;

namespace CleanArch.API.Configuration;

/// <summary>
/// Configuration options for Keycloak authentication and authorization.
/// </summary>
public class KeycloakOptions
{
    /// <summary>
    /// The configuration section name for Keycloak settings.
    /// </summary>
    public const string SectionName = "Keycloak";

    /// <summary>
    /// Gets or sets the Keycloak authority URL (e.g., "http://localhost:8080/realms/cleanarch").
    /// This is the base URL of the Keycloak realm used for token validation.
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience claim value expected in JWT tokens.
    /// This should match the client ID configured in Keycloak (e.g., "cleanarch-api").
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether HTTPS is required when retrieving metadata from the authority.
    /// Set to <c>false</c> for development environments using HTTP.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of OAuth2 scopes required for accessing the API.
    /// These scopes should match the scopes configured in Keycloak for the client.
    /// </summary>
    public List<string> Scopes { get; set; } = new();
}

/// <summary>
/// Extension methods for loading <see cref="KeycloakOptions"/> from configuration.
/// </summary>
public static class KeycloakOptionsExtensions
{
    /// <summary>
    /// Loads and validates Keycloak configuration options from the configuration section.
    /// </summary>
    /// <param name="configuration">The configuration manager to read from.</param>
    /// <returns>A validated <see cref="KeycloakOptions"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration section is missing or cannot be bound.</exception>
    public static KeycloakOptions LoadKeycloakOptions(this IConfigurationManager configuration)
    {
        var keycloakOptions = configuration.GetRequiredSection(KeycloakOptions.SectionName).Get<KeycloakOptions>()
                              ?? throw new InvalidOperationException($"Configuration section '{KeycloakOptions.SectionName}' is required.");
        return keycloakOptions;
    }
}

