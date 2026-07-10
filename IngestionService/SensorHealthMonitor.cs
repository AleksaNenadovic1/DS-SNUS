using IngestionService.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace IngestionService.Background;


public class SensorHealthMonitor : BackgroundService
{

    private readonly IServiceScopeFactory _scopeFactory;


    public SensorHealthMonitor(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }



    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {

            try
            {

                using var scope =
                    _scopeFactory.CreateScope();


                var context =
                    scope.ServiceProvider
                    .GetRequiredService<ScadaDbContext>();



                await CheckSensors(context);


            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Sensor monitor error: {ex.Message}"
                );
            }



            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken
            );

        }

    }

    private async Task CheckSensors(
        ScadaDbContext context)
    {
        var sensors =
            await context.Sensors.ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var sensor in sensors)
        {

            if (sensor.IsBlocked &&
                sensor.BlockedUntil <= now)
            {
                sensor.IsBlocked = false;

                Console.WriteLine(
                    $"Sensor {sensor.Id} block expired."
                );
            }

            if (
                sensor.IsActive &&
                sensor.LastSeen != DateTime.MinValue &&
                now - sensor.LastSeen
                >
                TimeSpan.FromSeconds(10)
            )
            {

                sensor.IsActive = false;
                sensor.Quality = 
                    SensorQuality.BAD;
                sensor.Behavior =
                    SensorBehavior.Offline;

                Console.WriteLine(
                    $"Sensor {sensor.Id} became inactive."
                );

                await EventLogger.LogAsync(
                    context,
                    sensor.Id,
                    "SENSOR_TIMEOUT",
                    "Sensor became inactive due to timeout."
                );

            }

        }

        int activeCount =
            sensors.Count(
                s => s.IsActive);

        if (activeCount < 5)
        {
            var replacements =
                sensors
                .Where(s =>
                    !s.IsActive &&
                    !s.IsBlocked)
                .OrderBy(s =>
                    s.Quality == SensorQuality.BAD)
                .Take(5 - activeCount);

            foreach (var sensor in replacements)
            {

                sensor.IsActive = true;

                sensor.LastSeen = DateTime.UtcNow;

                Console.WriteLine(
                    $"Sensor {sensor.Id} activated as replacement."
                );

                await EventLogger.LogAsync(
                    context,
                    sensor.Id,
                    "SENSOR_ACTIVATED",
                    "Sensor activated as replacement."
                );

            }

        }

        await context.SaveChangesAsync();

    }

}