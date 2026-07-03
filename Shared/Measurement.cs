using Shared.Enums;

namespace Shared.Models;

public class Measurement
{
    public long Id { get; set; }

    public int SensorId { get; set; }

    public Sensor Sensor { get; set; }

    public double Temperature { get; set; }

    public DateTime Timestamp { get; set; }

    public AlarmPriority AlarmPriority { get; set; }

    public SensorQuality Quality { get; set; }

    public bool IsConsensus { get; set; }
}