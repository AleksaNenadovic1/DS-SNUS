using IngestionService.Data;
using Microsoft.EntityFrameworkCore;

namespace IngestionService.Background;

public class SensorStartupInitializer : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;


    public SensorStartupInitializer(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }



    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();


        var context =
            scope.ServiceProvider
            .GetRequiredService<ScadaDbContext>();


        var sensors =
            await context.Sensors
            .Where(s => s.IsActive)
            .ToListAsync();



        foreach (var sensor in sensors)
        {
            sensor.LastSeen =
                DateTime.UtcNow;
        }


        await context.SaveChangesAsync();


        Console.WriteLine(
            "Sensor startup timestamps initialized."
        );
    }



    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}