using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Storage paths are configurable via appsettings or environment variables
        // (Storage__AppDataPath / Storage__MediaPath). In Docker these point at the
        // /appdata and /media volume mounts.
        var appDataPath = Path.GetFullPath(builder.Configuration["Storage:AppDataPath"] ?? "appdata");
        Directory.CreateDirectory(appDataPath);

        var dbPath = Path.Combine(appDataPath, "medialibraryoptimizer.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddOpenApi();

        // The Vite dev server runs on a different origin during development.
        builder.Services.AddCors(options =>
            options.AddPolicy("DevFrontend", policy =>
                policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

        var app = builder.Build();

        // Apply pending EF Core migrations on startup so the SQLite database in the
        // appdata volume is always up to date without manual intervention.
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseCors("DevFrontend");
        }

        // Serve the built frontend (copied into wwwroot in the Docker image).
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapControllers();

        // SPA fallback: any non-API route is handled client-side by React.
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
