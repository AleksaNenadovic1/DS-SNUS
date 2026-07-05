namespace Shared.Dto;

using System.ComponentModel.DataAnnotations.Schema;

[Table("sensor_ingest_dtos")]
public class SensorIngestDto
{
    public int SensorId { get; set; }

    public double Value { get; set; }

    public DateTime Timestamp { get; set; }

}