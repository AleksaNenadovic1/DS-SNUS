using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;
using Shared.Dto;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly IHubContext<AlarmHub> _hub;

    public NotificationController(
        IHubContext<AlarmHub> hub)
    {
        _hub = hub;
    }

    [HttpPost("alarm")]
    public async Task<IActionResult> SendAlarm(
        AlarmNotification notification)
    {
        await _hub.Clients.All.SendAsync(
            "AlarmReceived",
            notification);

        Console.WriteLine(
            $"Forwarded alarm from sensor {notification.SensorId}");

        return Ok();
    }
}