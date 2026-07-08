using System.Net.Http.Json;
using Shared.Dto;
using Shared.Enums;


var http = new HttpClient();

var baseUrl =
    "http://localhost:5141/api/ingest/sensor";


var random = new Random();


Console.WriteLine("Sensor simulator started...");


while (true)
{

    var sensors =
        await http.GetFromJsonAsync<List<ActiveSensorDto>>
        (
            "http://localhost:5141/api/ingest/sensors/active"
        )
        ?? new List<ActiveSensorDto>();


    foreach (var sensor in sensors)
    {

        //double value =
        //    sensor.MinTemperature +
        //    random.NextDouble()
        //    *
        //    (sensor.MaxTemperature - sensor.MinTemperature);

        double value = random.NextDouble() * 100; // Simulate a random temperature value between 0 and 100



        AlarmPriority priority = AlarmPriority.None;


        if (value <= sensor.MinTemperature - sensor.Alarm3Limit ||
            value >= sensor.MaxTemperature + sensor.Alarm3Limit)
        {
            priority = AlarmPriority.High;
        }
        else if (value <= sensor.MinTemperature - sensor.Alarm2Limit ||
                 value >= sensor.MaxTemperature + sensor.Alarm2Limit)
        {
            priority = AlarmPriority.Medium;
        }
        else if (value <= sensor.MinTemperature - sensor.Alarm1Limit ||
                 value >= sensor.MaxTemperature + sensor.Alarm1Limit)
        {
            priority = AlarmPriority.Low;
        }

        PrintValue(sensor.Id, value, priority);



        var dto = new SensorIngestDto
        {
            SensorId = sensor.Id,

            Value = value,

            Timestamp = DateTime.UtcNow,

            AlarmPriority = priority,

            Quality = sensor.Quality
        };



        try
        {

            var response =
                await http.PostAsJsonAsync(baseUrl, dto);


            //Console.WriteLine(
            //    $"Server response: {response.StatusCode}"
            //);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }


    await Task.Delay(2000);

}



static void PrintValue(
    int sensorId,
    double value,
    AlarmPriority priority)
{

    ConsoleColor old =
        Console.ForegroundColor;


    switch (priority)
    {
        case AlarmPriority.Low:
            Console.ForegroundColor = ConsoleColor.Yellow;
            break;


        case AlarmPriority.Medium:
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            break;


        case AlarmPriority.High:
            Console.ForegroundColor = ConsoleColor.Red;
            break;


        default:
            Console.ForegroundColor = ConsoleColor.White;
            break;
    }


    Console.WriteLine(
        $"Sensor {sensorId}: {value:F2}°C Alarm:{priority}"
    );


    Console.ForegroundColor = old;
}