using Shared.Dto;
using System.Collections.Concurrent;
using System.Net.Http.Json;


namespace SensorSimulator;


class Program
{

    private static readonly HttpClient http = new();

    //private const string SensorsUrl =
    //    "http://localhost:5141/api/ingest/sensors/active";

    private const string SensorsUrl =
        "http://ingestion:8080/api/ingest/sensors/active";



    private static readonly ConcurrentDictionary<int, SensorState>
        Sensors = new();



    static async Task Main()
    {

        Console.WriteLine(
            "Sensor simulator started..."
        );


        Console.WriteLine();

        Console.WriteLine(
            "Commands:"
        );

        Console.WriteLine(
            "block <sensorId>"
        );

        Console.WriteLine();



        using var cancellation =
            new CancellationTokenSource();



        // Start command listener

        _ =
            Task.Run(
                () => CommandLoop(
                    cancellation.Token
                )
            );



        while (!cancellation.Token.IsCancellationRequested)
        {

            try
            {

                await LoadSensors();



            }
            catch (Exception ex)
            {

                Console.WriteLine(
                    $"Sensor loading error: {ex.Message}"
                );

            }



            await Task.Delay(
                TimeSpan.FromSeconds(10),
                cancellation.Token
            );

        }

    }





    private static async Task LoadSensors()
    {

        var sensors =
            await http.GetFromJsonAsync<List<ActiveSensorDto>>
            (
                SensorsUrl
            )
            ??
            new List<ActiveSensorDto>();

        var activeIds =
            sensors
            .Select(s => s.Id)
            .ToHashSet();

        foreach (var existing in Sensors)
        {

            if (!activeIds.Contains(existing.Key))
            {

                Console.WriteLine(
                    $"Stopping sensor {existing.Key}"
                );


                existing.Value.Running = false;


                Sensors.TryRemove(
                    existing.Key,
                    out _
                );

            }

        }

        foreach (var sensor in sensors)
        {

            if (Sensors.TryGetValue(sensor.Id, out var existing))
            {
                existing.Sensor.Quality = sensor.Quality;
                existing.Sensor.MinTemperature = sensor.MinTemperature;
                existing.Sensor.MaxTemperature = sensor.MaxTemperature;

                continue;
            }

            if (!Sensors.ContainsKey(sensor.Id))
            {

                var state =
                    new SensorState(sensor);



                Sensors.TryAdd(
                    sensor.Id,
                    state
                );



                Console.WriteLine(
                    $"Starting sensor {sensor.Id}"
                );



                _ =
                    Task.Run(
                        () =>
                            SensorWorker.RunAsync(
                                state
                            )
                    );

            }

        }


    }





    private static async Task CommandLoop(
        CancellationToken token)
    {

        while (!token.IsCancellationRequested)
        {

            var command =
                Console.ReadLine();



            if (string.IsNullOrWhiteSpace(command))
                continue;



            var parts =
                command.Split(' ');



            if (parts.Length != 2)
                continue;



            if (!int.TryParse(
                parts[1],
                out int sensorId))
            {
                continue;
            }



            switch (parts[0].ToLower())
            {

                case "block":

                    await BlockSensor(sensorId);

                    break;



                default:

                    Console.WriteLine(
                        "Unknown command"
                    );

                    break;

            }

        }

    }





    private static async Task BlockSensor(
        int sensorId)
    {

        try
        {

            //var response =
            //    await http.PostAsync(
            //        $"http://localhost:5141/api/ingest/sensor/{sensorId}/block",
            //        null
            //    );

            var response =
                await http.PostAsync(
                    $"http://ingestion:8080/api/ingest/sensor/{sensorId}/block",
                    null
                );



            if (response.IsSuccessStatusCode)
            {

                Console.WriteLine(
                    $"Sensor {sensorId} blocked."
                );

            }
            else
            {

                Console.WriteLine(
                    $"Blocking failed: {response.StatusCode}"
                );

            }

        }
        catch (Exception ex)
        {

            Console.WriteLine(
                $"Block error: {ex.Message}"
            );

        }

    }

}