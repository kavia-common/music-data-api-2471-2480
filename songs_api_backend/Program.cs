using Microsoft.EntityFrameworkCore;
using songs_api_backend.Data;
using songs_api_backend.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Music Data API";
    settings.Version = "v1";
    settings.Description = "RESTful API for songs, artists, and genres. Supports pagination and filtering by artist and genre.";
});

// Configure EF Core with provider based on environment variables.
// Env vars (set via .env file by the orchestrator):
// - DATABASE_PROVIDER: sqlite | sqlserver | postgres (default: sqlite)
// - DATABASE_CONNECTION_STRING: full provider connection string (required for sqlserver/postgres)
// - SQLITE_PATH: relative or absolute file path for SQLite (optional; default: Data/music.db)
var configuration = builder.Configuration;
var env = builder.Environment;
var provider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER")?.ToLowerInvariant() ?? "sqlite";

builder.Services.AddDbContext<MusicDbContext>(options =>
{
    switch (provider)
    {
        case "sqlserver":
            {
                var cs = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
                if (string.IsNullOrWhiteSpace(cs))
                    throw new InvalidOperationException("DATABASE_CONNECTION_STRING must be set when DATABASE_PROVIDER=sqlserver");
                options.UseSqlServer(cs);
                break;
            }
        case "postgres":
        case "postgresql":
            {
                var cs = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
                if (string.IsNullOrWhiteSpace(cs))
                    throw new InvalidOperationException("DATABASE_CONNECTION_STRING must be set when DATABASE_PROVIDER=postgres");
                options.UseNpgsql(cs);
                break;
            }
        case "sqlite":
        default:
            {
                // Default to local SQLite file for development/demo.
                var sqlitePath = Environment.GetEnvironmentVariable("SQLITE_PATH");
                if (string.IsNullOrWhiteSpace(sqlitePath))
                {
                    sqlitePath = Path.Combine(AppContext.BaseDirectory, "Data", "music.db");
                }
                var dir = Path.GetDirectoryName(sqlitePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                options.UseSqlite($"Data Source={sqlitePath}");
                break;
            }
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Health check endpoint
app.MapGet("/", () => new { message = "Healthy" })
   .WithName("HealthCheck")
   .WithSummary("Service health check")
   .WithDescription("Returns a simple healthy status.");

app.MapMusicApi();

// Ensure DB is created and seed demo data (safe for dev/demo)
// Note: In production, prefer EF Core migrations instead of EnsureCreated/seed.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
    await DbInitializer.InitializeAsync(db, logger);
}

app.Run();