using Shared.Enums;

namespace Shared.Dto;

public class ActiveSensorDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public double MinTemperature { get; set; }

    public double MaxTemperature { get; set; }

    public double Alarm1Limit { get; set; }

    public double Alarm2Limit { get; set; }

    public double Alarm3Limit { get; set; }

    public SensorQuality Quality { get; set; }
}