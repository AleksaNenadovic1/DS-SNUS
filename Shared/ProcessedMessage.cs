namespace Shared.Models;

using System.ComponentModel.DataAnnotations.Schema;

[Table("processed_messages")]
public class ProcessedMessage
{
    public long Id { get; set; }

    public int SensorId { get; set; }

    public long MessageId { get; set; }

    public DateTime Timestamp { get; set; }

}