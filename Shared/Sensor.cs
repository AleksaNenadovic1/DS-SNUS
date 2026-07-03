using Shared.Enums;
namespace Shared.Models;

public class Sensor
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public bool IsActive { get; set; }

    public SensorQuality Quality { get; set; }

    public double MinTemperature { get; set; }

    public double MaxTemperature { get; set; }

    public double Alarm1Limit { get; set; }

    public double Alarm2Limit { get; set; }

    public double Alarm3Limit { get; set; }

    public DateTime LastSeen { get; set; }

    public ICollection<Measurement> Measurements { get; set; }
        = new List<Measurement>();
}