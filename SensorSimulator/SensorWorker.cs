using Shared;
using Shared.Dto;
using Shared.Enums;
using Shared.Security;
using System.Net.Http.Json;
using System.Text.Json;


namespace SensorSimulator;


public static class SensorWorker
{

    private static readonly HttpClient http = new();



    private const string IngestUrl =
        "http://localhost:5141/api/ingest/sensor";



    public static async Task RunAsync(
    SensorState state)
    {

        var sensor =
            state.Sensor;


        Console.WriteLine(
            $"Sensor {sensor.Id} started. " +
            $"Quality={sensor.Quality}, " +
            $"Behavior={sensor.Behavior}"
        );


        while (state.Running)
        {

            try
            {

                await ExecuteCycle(state);

            }
            catch (Exception ex)
            {

                Console.WriteLine(
                    $"Sensor {sensor.Id} error: {ex.Message}"
                );

            }


            if (!state.Running)
                break;


            await DelayNextCycle(state);

        }


        Console.WriteLine(
            $"Sensor {sensor.Id} stopped."
        );

    }





    private static async Task ExecuteCycle(
        SensorState state)
    {

        var sensor =
            state.Sensor;

        /*
         * Offline sensor:
         * simply stops sending messages.
         */
        if (sensor.Behavior ==
            SensorBehavior.Offline)
        {

            return;

        }

        /*
         * Delayed malicious sensor.
         */
        if (sensor.Behavior ==
            SensorBehavior.Delayed)
        {

            await Task.Delay(
                TimeSpan.FromSeconds(
                    Random.Shared.Next(10, 20)
                )
            );

        }

        /*
         * Generate measurement.
         */
        double value =
            GenerateValue(sensor);



        /*
         * Malicious wrong values.
         */
        if (sensor.Behavior ==
            SensorBehavior.WrongValues)
        {

            value =
                Random.Shared.NextDouble()
                * 200;

        }





        AlarmPriority alarm =
            DetermineAlarm(
                value,
                sensor
            );



        var dto =
            new SensorIngestDto
            {

                SensorId =
                    sensor.Id,


                Value =
                    value,


                Timestamp =
                    DateTime.UtcNow,


                AlarmPriority =
                    alarm,


                Quality =
                    sensor.Quality,


                MessageId =
                    state.NextMessageId(),


                SentAt =
                    DateTime.UtcNow

            };





        /*
         * DoS simulation:
         *
         * Sends >10 messages/sec
         * so server rate limiter
         * blocks this sensor.
         */
        if (sensor.Behavior ==
            SensorBehavior.Dos)
        {

            var tasks =
               new List<Task>();


            for (int i = 0; i < 20; i++)
            {

                tasks.Add(
                    Send(dto)
                );

            }


            await Task.WhenAll(tasks);


            return;

        }





        await Send(dto);

    }







    private static async Task Send(
        SensorIngestDto dto)
    {

        try
        {

            string json =
                JsonSerializer.Serialize(dto);



            var encrypted =
                AesEncryption.Encrypt(json);



            var secure =
                new SecureMessageDto
                {

                    Data =
                        encrypted.Data,


                    IV =
                        encrypted.IV,


                    Signature =
                        EcdsaSignature.Sign(json)

                };



            var response =
                await http.PostAsJsonAsync(
                    IngestUrl,
                    secure
                );



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
                    $"Alarm {result.AlarmPriority}"
                );

            }
            else
            {

                var error =
                    await response.Content
                    .ReadAsStringAsync();


                Console.WriteLine(
                    $"SERVER REJECTED | " +
                    $"Sensor {dto.SensorId} | " +
                    $"{response.StatusCode}: {error}"
                );


                if (response.StatusCode ==
                    System.Net.HttpStatusCode.TooManyRequests)
                {

                    Console.WriteLine(
                        $"Sensor {dto.SensorId} was blocked by server."
                    );

                }

            }

        }
        catch (Exception ex)
        {

            Console.WriteLine(
                $"Send failed for sensor {dto.SensorId}: {ex.Message}"
            );

        }

    }







    private static double GenerateValue(
        ActiveSensorDto sensor)
    {

        var random =
            Random.Shared;



        double normal =
            sensor.MinTemperature +
            random.NextDouble()
            *
            (
                sensor.MaxTemperature -
                sensor.MinTemperature
            );



        switch (sensor.Quality)
        {

            case SensorQuality.GOOD:

                return normal;



            case SensorQuality.UNCERTAIN:

                if (random.NextDouble() < 0.3)
                {

                    return random.NextDouble()
                        * 150;

                }


                return normal;




            case SensorQuality.BAD:

                return random.NextDouble()
                    * 200;




            default:

                return normal;

        }

    }








    private static AlarmPriority DetermineAlarm(
        double value,
        ActiveSensorDto sensor)
    {


        /*
         * Priority 3 overrides everything.
         */
        if (
            value <=
            sensor.MinTemperature
            -
            sensor.Alarm3Limit

            ||

            value >=
            sensor.MaxTemperature
            +
            sensor.Alarm3Limit
        )
        {

            return AlarmPriority.High;

        }



        if (
            value <=
            sensor.MinTemperature
            -
            sensor.Alarm2Limit

            ||

            value >=
            sensor.MaxTemperature
            +
            sensor.Alarm2Limit
        )
        {

            return AlarmPriority.Medium;

        }



        if (
            value <=
            sensor.MinTemperature
            -
            sensor.Alarm1Limit

            ||

            value >=
            sensor.MaxTemperature
            +
            sensor.Alarm1Limit
        )
        {

            return AlarmPriority.Low;

        }


        return AlarmPriority.None;

    }

    private static async Task DelayNextCycle(
        SensorState state)
    {

        int seconds =
            Random.Shared.Next(
                1,
                10
            );

        await Task.Delay(
            TimeSpan.FromSeconds(seconds)
        );

    }

}