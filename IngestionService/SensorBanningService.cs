using IngestionService.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace IngestionService.Data;

public static class SensorBanningService
{
    public static async Task<bool> BlockSensor(int id, ScadaDbContext context)
    {
        var sensor =
            await context.Sensors
            .FirstOrDefaultAsync(
                s => s.Id == id);

        if (sensor == null)
            return false;

        sensor.IsBlocked = true;

        sensor.IsActive = false;

        sensor.BlockedUntil =
            DateTime.UtcNow.AddSeconds(30);

        await context.SaveChangesAsync();

        Console.WriteLine(
            $"Sensor {id} blocked."
        );

        return true;
    }
}
