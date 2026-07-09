using Shared.Enums;

namespace Shared.Dto;

public class AlarmNotification
{
    public int SensorId { get; set; }

    public double Value { get; set; }

    public AlarmPriority Priority { get; set; }

    public DateTime Timestamp { get; set; }
}