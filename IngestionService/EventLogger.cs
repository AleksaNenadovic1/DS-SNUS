using Shared.Models;

namespace IngestionService.Data;

public static class EventLogger
{
    public static async Task LogAsync(
        ScadaDbContext context,
        int sensorId,
        string eventType,
        string description)
    {
        var log = new EventLog
        {
            SensorId = sensorId,

            Timestamp = DateTime.UtcNow,

            EventType = eventType,

            Description = description
        };


        context.EventLogs.Add(log);

        await context.SaveChangesAsync();
    }
}