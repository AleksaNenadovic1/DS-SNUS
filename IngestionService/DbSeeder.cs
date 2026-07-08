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

                MinTemperature = 15,
                MaxTemperature = 25,

                Alarm1Limit = 22,
                Alarm2Limit = 24,
                Alarm3Limit = 25,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 2,
                Name = "Sensor 2",

                MinTemperature = 20,
                MaxTemperature = 35,

                Alarm1Limit = 30,
                Alarm2Limit = 33,
                Alarm3Limit = 35,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 3,
                Name = "Sensor 3",

                MinTemperature = 25,
                MaxTemperature = 45,

                Alarm1Limit = 38,
                Alarm2Limit = 42,
                Alarm3Limit = 45,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 4,
                Name = "Sensor 4",

                MinTemperature = 30,
                MaxTemperature = 55,

                Alarm1Limit = 45,
                Alarm2Limit = 50,
                Alarm3Limit = 55,

                Quality = SensorQuality.UNCERTAIN,

                IsActive = true
            },


            new Sensor
            {
                Id = 5,
                Name = "Sensor 5",

                MinTemperature = 10,
                MaxTemperature = 30,

                Alarm1Limit = 25,
                Alarm2Limit = 28,
                Alarm3Limit = 30,

                Quality = SensorQuality.GOOD,

                IsActive = true
            },


            new Sensor
            {
                Id = 6,
                Name = "Sensor 6",

                MinTemperature = 35,
                MaxTemperature = 70,

                Alarm1Limit = 55,
                Alarm2Limit = 65,
                Alarm3Limit = 70,

                Quality = SensorQuality.GOOD,

                IsActive = false
            },


            new Sensor
            {
                Id = 7,
                Name = "Sensor 7",

                MinTemperature = 20,
                MaxTemperature = 40,

                Alarm1Limit = 32,
                Alarm2Limit = 36,
                Alarm3Limit = 40,

                Quality = SensorQuality.GOOD,

                IsActive = false
            },


            new Sensor
            {
                Id = 8,
                Name = "Sensor 8",

                MinTemperature = 15,
                MaxTemperature = 60,

                Alarm1Limit = 40,
                Alarm2Limit = 50,
                Alarm3Limit = 60,

                Quality = SensorQuality.BAD,

                IsActive = false
            },


            new Sensor
            {
                Id = 9,
                Name = "Sensor 9",

                MinTemperature = 40,
                MaxTemperature = 80,

                Alarm1Limit = 60,
                Alarm2Limit = 70,
                Alarm3Limit = 80,

                Quality = SensorQuality.GOOD,

                IsActive = false
            },


            new Sensor
            {
                Id = 10,
                Name = "Sensor 10",

                MinTemperature = 5,
                MaxTemperature = 15,

                Alarm1Limit = 12,
                Alarm2Limit = 14,
                Alarm3Limit = 15,

                Quality = SensorQuality.GOOD,

                IsActive = false
            }
        };


        foreach (var sensor in sensors)
        {
            sensor.LastSeen = DateTime.MinValue;
            sensor.IsBlocked = false;
        }


        context.Sensors.AddRange(sensors);

        await context.SaveChangesAsync();

        Console.WriteLine("Database seeded with 10 sensors.");
    }
}