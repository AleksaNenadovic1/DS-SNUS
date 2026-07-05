namespace Shared.Models;

using System.ComponentModel.DataAnnotations.Schema;

[Table("consensus_values")]
public class ConsensusValue
{

    public long Id { get; set; }

    public DateTime Timestamp { get; set; }

    public double Value { get; set; }

}