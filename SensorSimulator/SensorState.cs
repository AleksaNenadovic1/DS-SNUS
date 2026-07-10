using Shared.Dto;

namespace SensorSimulator;


public class SensorState
{

    public ActiveSensorDto Sensor { get; }

    public bool Running { get; set; } = true;

    public long MessageId { get; private set; }



    public SensorState(
        ActiveSensorDto sensor)
    {

        Sensor = sensor;


        MessageId =
            DateTimeOffset.UtcNow
            .ToUnixTimeMilliseconds();

    }



    public long NextMessageId()
    {

        return MessageId++;

    }

}