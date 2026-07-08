using Shared.Dto;
using Shared.Enums;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Threading.Tasks;


var http = new HttpClient();


var baseUrl =
    "http://localhost:5141/api/ingest/sensor";


var activeSensorsUrl =
    "http://localhost:5141/api/ingest/sensors/active";


var random = new Random();


var commandQueue =
    new ConcurrentQueue<string>();

// blocked sensor -> unblock time
var blockedSensors =
    new Dictionary<int, DateTime>();


Console.WriteLine(
    "Sensor simulator started..."
);


Console.WriteLine(
    "Commands:"
);

Console.WriteLine(
    "block <sensorId>"
);

Console.WriteLine(
    "unblock <sensorId>"
);

// Console command listener
_ = Task.Run(() =>
{
    while (true)
    {
        var command = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(command))
        {
            commandQueue.Enqueue(command);
        }
    }
});


while (true)
{
    while (commandQueue.TryDequeue(out var command))
    {
        HandleCommand(
            command,
            blockedSensors,
            http
        );
    }

    try
    {
        var sensors =
            await http.GetFromJsonAsync<List<ActiveSensorDto>>(
                activeSensorsUrl)
            ??
            new List<ActiveSensorDto>();

        foreach (var sensor in sensors)
        {

            // blocked simulation

            if (blockedSensors.ContainsKey(sensor.Id))
            {

                if (blockedSensors[sensor.Id] >
                    DateTime.UtcNow)
                {

                    Console.WriteLine(
                        $"Sensor {sensor.Id} is blocked."
                    );

                    continue;

                }
                else
                {

                    blockedSensors.Remove(sensor.Id);

                }

            }

            // Generate temperature according to sensor range

            var temperature =
                sensor.MinTemperature +
                random.NextDouble()
                *
                (
                    sensor.MaxTemperature -
                    sensor.MinTemperature
                );

            var alarm =
                DetermineAlarm(
                    temperature,
                    sensor
                );

            var dto =
                new SensorIngestDto
                {

                    SensorId = sensor.Id,

                    Value = temperature,

                    Timestamp =
                        DateTime.UtcNow,

                    AlarmPriority = alarm,

                    Quality =
                        sensor.Quality

                };

            var response =
                await http.PostAsJsonAsync(
                    baseUrl,
                    dto);

            if (response.IsSuccessStatusCode)
            {

                PrintSensorValue(
                    sensor,
                    temperature,
                    alarm
                );

            }
            else
            {

                Console.WriteLine(
                    $"Sensor {sensor.Id} failed: {response.StatusCode}"
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

static AlarmPriority DetermineAlarm(
    double value,
    ActiveSensorDto sensor)
{

    // Highest priority wins

    if (value >= sensor.Alarm3Limit)
        return AlarmPriority.High;


    if (value >= sensor.Alarm2Limit)
        return AlarmPriority.Medium;


    if (value >= sensor.Alarm1Limit)
        return AlarmPriority.Low;


    return AlarmPriority.None;

}

static void PrintSensorValue(
    ActiveSensorDto sensor,
    double value,
    AlarmPriority alarm)
{

    if (alarm == AlarmPriority.None)
    {

        Console.WriteLine(
            $"Sensor {sensor.Id}: {value:F2}°C"
        );

        return;

    }



    Console.ForegroundColor =
        alarm switch
        {

            AlarmPriority.Low =>
                ConsoleColor.Yellow,


            AlarmPriority.Medium =>
                ConsoleColor.DarkYellow,


            AlarmPriority.High =>
                ConsoleColor.Red,


            _ =>
                ConsoleColor.White
        };



    Console.WriteLine(
        $"ALARM {alarm}: Sensor {sensor.Id}, Value {value:F2}°C"
    );


    Console.ResetColor();

}

static async Task HandleCommand(
    string command,
    Dictionary<int, DateTime> blockedSensors,
    HttpClient http)
{
    var parts =
        command.Split(' ');

    if (parts.Length != 2)
        return;

    if (!int.TryParse(parts[1], out int sensorId))
        return;

    switch (parts[0].ToLower())
    {
        case "block":

            Task block = http.PostAsync(
                $"http://localhost:5141/api/ingest/sensor/{sensorId}/block",
                null
            );

            await block;

            Console.WriteLine(
                $"Requested block for sensor {sensorId}"
            );

            break;

        default:

            Console.WriteLine(
                "Unknown command."
            );

            break;
    }

}