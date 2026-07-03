namespace Shared.Dto;

public class SensorIngestDto
{
    public int SensorId { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}