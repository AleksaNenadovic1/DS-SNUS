using Shared.Enums;
using Shared.Models;

namespace IngestionService.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ScadaDbContext context)
    {
        if (context.Sensors.Any())
            return;


        var sensors = new List<Sensor>
        {
            new Sensor
            {
                Id = 1,
                Name = "Sensor 1",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 2,
                Name = "Sensor 2",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 3,
                Name = "Sensor 3",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 4,
                Name = "Sensor 4",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.UNCERTAIN,

                Behavior = SensorBehavior.WrongValues,

                IsActive = true
            },


            new Sensor
            {
                Id = 5,
                Name = "Sensor 5",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 6,
                Name = "Sensor 6",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,
                
                Behavior = SensorBehavior.Delayed,

                IsActive = false
            },


            new Sensor
            {
                Id = 7,
                Name = "Sensor 7",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                Behavior = SensorBehavior.Offline,

                IsActive = false
            },


            new Sensor
            {
                Id = 8,
                Name = "Sensor 8",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                Behavior = SensorBehavior.Dos,

                IsActive = false
            },


            new Sensor
            {
                Id = 9,
                Name = "Sensor 9",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                IsActive = false
            },


            new Sensor
            {
                Id = 10,
                Name = "Sensor 10",

                MinTemperature = 30,
                MaxTemperature = 70,

                Alarm1Limit = 0,
                Alarm2Limit = 10,
                Alarm3Limit = 20,

                Quality = SensorQuality.GOOD,

                IsActive = false
            }
        };


        foreach (var sensor in sensors)
        {
            sensor.LastSeen =
                sensor.IsActive
                    ? DateTime.UtcNow
                    : DateTime.MinValue;
            sensor.IsBlocked = false;
        }


        context.Sensors.AddRange(sensors);

        await context.SaveChangesAsync();

        Console.WriteLine("Database seeded with 10 sensors.");
    }
}