using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using CleanArch.API.Configuration;

namespace CleanArch.API;

public class Api
{
    public Api(WebApplicationBuilder builder)
    {
        Builder = builder;
    }

    public WebApplicationBuilder Builder { get; }

    public async Task RunAsync()
    {
        Configuration();
        ConfigureServices();
        var app = Builder.Build();
        Apply(app);
        await app.RunAsync();
    }

    protected void Configuration()
    {
        Builder.Configuration.AddJsonFile("appsettings.json");
        Builder.Configuration.AddJsonFile($"appsettings.{Builder.Environment.EnvironmentName}.json", optional: true);
        Builder.Configuration.AddUserSecrets(this.GetType().Assembly);
        Builder.Configuration.AddEnvironmentVariables();
    }

    protected void ConfigureServices()
    {
        AddControllers();
        AddAuthentication();
        AddSwagger();
    }

    protected void AddControllers()
    {
        Builder.Services.AddControllers();
    }

    protected void AddAuthentication()
    {
        var keycloakOptions = Builder.Configuration.LoadKeycloakOptions();

        if (string.IsNullOrWhiteSpace(keycloakOptions.Authority))
            throw new InvalidOperationException($"{KeycloakOptions.SectionName}:Authority is required.");
        if (string.IsNullOrWhiteSpace(keycloakOptions.Audience))
            throw new InvalidOperationException($"{KeycloakOptions.SectionName}:Audience is required.");

        Builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = keycloakOptions.Authority;
            options.Audience = keycloakOptions.Audience;
            options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        Builder.Services.AddAuthorization();
    }

    protected void AddSwagger()
    {
        var keycloakOptions = Builder.Configuration.LoadKeycloakOptions();

        Builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CleanArch API",
                Version = "v1",
                Description = "API with JWT Authentication via Keycloak"
            });

            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{keycloakOptions.Authority}/protocol/openid-connect/auth", UriKind.Absolute),
                        Scopes = new Dictionary<string, string>
                        {
                            ["email"] = "Email address",
                            ["profile"] = "Profile information"
                        }
                    }
                }
            });
        });
    }

    protected void Apply(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            var keycloakOptions = Builder.Configuration.LoadKeycloakOptions();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
              options.EnableTryItOutByDefault();
              options.DisplayRequestDuration();
              options.OAuthClientId(keycloakOptions.Audience);
              options.OAuthScopes(keycloakOptions.Scopes.ToArray());
              options.EnablePersistAuthorization();
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}
