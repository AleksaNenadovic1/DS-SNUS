using System.Net.Http.Json;
using Shared.Dto;

var http = new HttpClient();

// change this to your real port
var baseUrl = "https://localhost:7217/api/ingest/sensor";

// simulate 5 sensors
var sensors = Enumerable.Range(1, 5).ToList();

var random = new Random();

Console.WriteLine("Sensor simulator started...");

while (true)
{

    foreach (var sensorId in sensors)
    {

        var dto = new SensorIngestDto
        {

            SensorId = sensorId,
            Value = 20 + random.NextDouble() * 10, // 20–30°C
            Timestamp = DateTime.UtcNow

        };

        try
        {

            var response = await http.PostAsJsonAsync(baseUrl, dto);

            if (response.IsSuccessStatusCode)
            {

                Console.WriteLine($"Sensor {sensorId} sent value {dto.Value:F2}");

            }

            else
            {

                Console.WriteLine($"Failed for sensor {sensorId}: {response.StatusCode}");

            }

        }

        catch (Exception ex)
        {

            Console.WriteLine($"Error sensor {sensorId}: {ex.Message}");

        }

    }

    await Task.Delay(2000); // every 2 seconds

}