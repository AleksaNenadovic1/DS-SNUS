using Shared.Enums;

namespace Shared.Dto;

public class IngestResponseDto
{
    public int SensorId { get; set; }

    public double Value { get; set; }

    public AlarmPriority AlarmPriority { get; set; }

    public SensorQuality Quality { get; set; }

    public DateTime Timestamp { get; set; }

    public string Message { get; set; } = "";
}