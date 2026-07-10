using IngestionService;
using IngestionService.Background;
using IngestionService.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Data;


var builder = WebApplication.CreateBuilder(args);


// Database

builder.Services.AddDbContext<ScadaDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("IngestionService")));



// Background services

builder.Services.AddHostedService<SensorHealthMonitor>();



// Normal services

builder.Services.AddControllers();

builder.Services.AddSingleton<SensorRateLimiter>();

builder.Services.AddOpenApi();



var app = builder.Build();




// Configure pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();





// ===============================
// DATABASE INITIALIZATION
// ===============================

using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
        .GetRequiredService<ScadaDbContext>();


    var retries = 10;


    while (retries > 0)
    {
        try
        {

            Console.WriteLine(
                "Applying database migrations..."
            );


            await db.Database.MigrateAsync();


            Console.WriteLine(
                "Database migration complete."
            );



            await DbSeeder.SeedAsync(db);


            Console.WriteLine(
                "Database seeding complete."
            );


            break;

        }
        catch (Exception ex)
        {

            Console.WriteLine(
                $"Database unavailable. Retrying... {ex.Message}"
            );


            retries--;


            await Task.Delay(
                TimeSpan.FromSeconds(5)
            );

        }

    }

}



// ===============================
// SENSOR STARTUP FIX
// ===============================

using (var scope = app.Services.CreateScope())
{

    var initializer =
        new SensorStartupInitializer(
            scope.ServiceProvider
            .GetRequiredService<IServiceScopeFactory>()
        );


    await initializer.StartAsync(
        CancellationToken.None
    );

}



app.Run();