using Shared;
using Shared.Dto;
using Shared.Enums;
using Shared.Security;
using System.Net.Http.Json;
using System.Text.Json;


var http = new HttpClient();

var baseUrl =
    "http://localhost:5141/api/ingest/sensor";

var sensorsUrl =
    "http://localhost:5141/api/ingest/sensors/active";

var random = new Random();

Console.WriteLine(
    "Sensor simulator started..."
);

Console.WriteLine();
Console.WriteLine("Commands:");
Console.WriteLine("block <sensorId>");
Console.WriteLine();


long messageId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

// command listener
_ = Task.Run(async () =>
{
    while (true)
    {
        var command =
            Console.ReadLine();

        if (string.IsNullOrWhiteSpace(command))
            continue;

        var parts =
            command.Split(' ');

        if (parts.Length != 2)
            continue;

        if (!int.TryParse(parts[1], out int sensorId))
            continue;

        switch (parts[0].ToLower())
        {
            case "block":
                try
                {
                    var response =
                        await http.PostAsync(
                            $"http://localhost:5141/api/ingest/sensor/{sensorId}/block",
                            null);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(
                            $"Sensor {sensorId} blocked."
                        );
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Failed blocking sensor {sensorId}: {response.StatusCode}"
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Block error: {ex.Message}"
                    );
                }
                break;

            default:
                Console.WriteLine(
                    "Unknown command."
                );
                break;
        }
    }
});

while (true)
{
    try
    {
        var sensors =
            await http.GetFromJsonAsync<List<ActiveSensorDto>>
            (
                sensorsUrl
            )
            ??
            new List<ActiveSensorDto>();

        foreach (var sensor in sensors)
        {
            double value =
                GenerateValue(sensor);

            AlarmPriority alarm =
                DetermineAlarm(
                    value,
                    sensor
                );

            var dto =
                new SensorIngestDto
                {
                    SensorId = sensor.Id,

                    Value = value,

                    Timestamp =
                        DateTime.UtcNow,

                    AlarmPriority = alarm,

                    Quality =
                        sensor.Quality,

                    MessageId = messageId++,

                    SentAt = DateTime.UtcNow
                };



            try
            {

                string json =
                    JsonSerializer.Serialize(dto);

                var encrypted =
                    AesEncryption.Encrypt(json);

                var secure =
                    new SecureMessageDto
                    {
                        Data = encrypted.Data,

                        IV = encrypted.IV,

                        Signature =
                            EcdsaSignature.Sign(json)
                    };

                var response =
                    await http.PostAsJsonAsync(
                        baseUrl,
                        secure);

                if (response.IsSuccessStatusCode)
                {
                    var result =
                        await response
                        .Content
                        .ReadFromJsonAsync<IngestResponseDto>();

                    Console.WriteLine(
                        $"SERVER ACCEPTED | " +
                        $"Sensor {result!.SensorId} | " +
                        $"Value {result.Value:F2}°C | " +
                        $"Alarm {result.AlarmPriority} | " +
                        $"Quality {result.Quality}"
                    );
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(
                        $"Server response: {response.StatusCode} - {error}"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Sending failed: {ex.Message}"
                );
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"Simulator error: {ex.Message}"
        );
    }

    await Task.Delay(2000);
}

static double GenerateValue(
    ActiveSensorDto sensor)
{
    var random =
        Random.Shared;

    switch (sensor.Quality)
    {
        // Normal operation
        case SensorQuality.GOOD:

            return sensor.MinTemperature +
                random.NextDouble()
                *
                (
                    sensor.MaxTemperature -
                    sensor.MinTemperature
                );

        // Sometimes produces bad values
        case SensorQuality.UNCERTAIN:
            if (random.NextDouble() < 0.3)
            {
                return random.NextDouble() * 100;
            }

            return sensor.MinTemperature +
                random.NextDouble()
                *
                (
                    sensor.MaxTemperature -
                    sensor.MinTemperature
                );

        // Broken sensor
        case SensorQuality.BAD:
            return random.NextDouble() * 150;

        default:
            return 0;
    }
}

static AlarmPriority DetermineAlarm(
    double value,
    ActiveSensorDto sensor)
{
    if ((value <= sensor.MinTemperature - sensor.Alarm3Limit) || (value >= sensor.MaxTemperature + sensor.Alarm3Limit))
        return AlarmPriority.High;

    if ((value <= sensor.MinTemperature - sensor.Alarm2Limit) || (value >= sensor.MaxTemperature + sensor.Alarm2Limit))
        return AlarmPriority.Medium;

    if ((value <= sensor.MinTemperature - sensor.Alarm1Limit) || (value >= sensor.MaxTemperature + sensor.Alarm1Limit))
        return AlarmPriority.Low;

    return AlarmPriority.None;

}