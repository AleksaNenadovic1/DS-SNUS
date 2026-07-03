namespace Shared.Models;

public class EventLog
{
    public long Id { get; set; }

    public DateTime Timestamp { get; set; }

    public int SensorId { get; set; }

    public string EventType { get; set; } = "";

    public string Description { get; set; } = "";
}