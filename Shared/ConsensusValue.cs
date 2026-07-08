namespace Shared.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class ConsensusValue
{

    public long Id { get; set; }

    public DateTime Timestamp { get; set; }

    public double Value { get; set; }

}