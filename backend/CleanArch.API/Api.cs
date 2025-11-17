using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using CleanArch.API.Configuration;
using Microsoft.Extensions.Logging;
using CleanArch.API.Middleware;
using CleanArch.API.Extensions;
using CleanArch.API.Services;
using CleanArch.Core;
using CleanArch.Logging;

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
        Prebuild();
        var app = Builder.Build();
        Apply(app);
        await app.RunAsync();
    }

    #region PreBuild

    protected void Prebuild()
    {
        Configuration();
        ConfigureServices();
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
        AddLogging();
        AddOpenTelemetry();
        AddControllers();
        AddAuthentication();
        AddCurrentUserService();
        AddSwagger();
    }

    protected void AddLogging()
    {
        Builder.Services.AddCleanLogging(Builder.Configuration);
    }

    protected void AddOpenTelemetry()
    {
        var openTelemetryOptions = Builder.Configuration.LoadOpenTelemetryOptions();
        Builder.Services.AddOpenTelemetry(openTelemetryOptions);
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

    protected void AddCurrentUserService()
    {
        // Register IHttpContextAccessor if not already registered
        Builder.Services.AddHttpContextAccessor();
        
        // Register ICurrentUserService as scoped (one instance per HTTP request)
        Builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    }

    protected void AddSwagger()
    {
        var keycloakOptions = Builder.Configuration.LoadKeycloakOptions();

        Builder.Services.AddSwaggerGen(options =>
        {
          options.SwaggerDoc("v1", new OpenApiInfo
          {
            Title = "CleanArch API",
            Version = "v1",
            Description = "API with JWT Authentication via Keycloak"
          });

          options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
          {
            Type = SecuritySchemeType.OAuth2,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Flows = new OpenApiOAuthFlows
            {
              Implicit = new OpenApiOAuthFlow
              {
                AuthorizationUrl = new Uri($"{keycloakOptions.Authority}/protocol/openid-connect/auth", UriKind.Absolute),
                Scopes = keycloakOptions.Scopes.ToDictionary(scope => scope, scope => scope)
              }
            }
          });
          
          options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
          {
              [new OpenApiSecuritySchemeReference("oauth2", document)] = keycloakOptions.Scopes
          });
        });
    }

    #endregion

    #region PostBuild
    
    protected void Apply(WebApplication app)
    {
        // Exception handling middleware should be after Swagger but before controllers
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
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
    
    #endregion
}
