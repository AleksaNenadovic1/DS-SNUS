namespace Shared.Dto;

using Shared.Enums;

public class SensorIngestDto
{
    public int SensorId { get; set; }

    public double Value { get; set; }

    public DateTime Timestamp { get; set; }

    public AlarmPriority AlarmPriority { get; set; }

    public SensorQuality Quality { get; set; }
}