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
        Builder.Services.AddSwaggerGen();
    }

    protected void AddControllers()
    {
        Builder.Services.AddControllers();
    }

    protected void Apply(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.EnableTryItOutByDefault();
                c.DisplayRequestDuration();
            });
        }

        app.UseHttpsRedirection();

        app.MapControllers();
    }
}
