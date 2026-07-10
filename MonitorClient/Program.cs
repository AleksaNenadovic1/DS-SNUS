using Microsoft.AspNetCore.SignalR.Client;
using Shared.Dto;
using Shared.Enums;

Console.Title = "SCADA Monitor Client";

Console.WriteLine("Connecting to NotificationService...");

//var connection =
//    new HubConnectionBuilder()
//        .WithUrl("http://localhost:5068/alarmHub")
//        .WithAutomaticReconnect()
//        .Build();

var connection =
    new HubConnectionBuilder()
        .WithUrl("http://notification:8080/alarmHub")
        .WithAutomaticReconnect()
        .Build();

connection.Reconnecting += error =>
{
    Console.ForegroundColor = ConsoleColor.Yellow;

    Console.WriteLine("Connection lost... reconnecting.");

    Console.ResetColor();

    return Task.CompletedTask;
};

connection.Reconnected += id =>
{
    Console.ForegroundColor = ConsoleColor.Green;

    Console.WriteLine("Reconnected.");

    Console.ResetColor();

    return Task.CompletedTask;
};

connection.Closed += error =>
{
    Console.ForegroundColor = ConsoleColor.Red;

    Console.WriteLine("Disconnected.");

    Console.ResetColor();

    return Task.CompletedTask;
};

connection.On<AlarmNotification>(
    "AlarmReceived",
    alarm =>
    {
        Console.ForegroundColor =
            alarm.Priority switch
            {
                AlarmPriority.Low => ConsoleColor.Yellow,

                AlarmPriority.Medium => ConsoleColor.DarkYellow,

                AlarmPriority.High => ConsoleColor.Red,

                _ => ConsoleColor.White
            };

        Console.WriteLine();

        Console.WriteLine("==============================");

        Console.WriteLine($"Time      : {alarm.Timestamp:HH:mm:ss}");

        Console.WriteLine($"Sensor ID : {alarm.SensorId}");

        Console.WriteLine($"Priority  : {alarm.Priority}");

        Console.WriteLine($"Value     : {alarm.Value:F2} °C");

        Console.WriteLine("==============================");

        Console.ResetColor();
    });

await connection.StartAsync();

Console.ForegroundColor = ConsoleColor.Green;

Console.WriteLine("Connected.");

Console.ResetColor();

Console.WriteLine();

Console.WriteLine("Waiting for alarms...");

await Task.Delay(Timeout.Infinite);