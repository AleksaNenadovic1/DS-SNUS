using IngestionService;
using IngestionService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Dto;
using Shared.Enums;
using Shared.Models;
using Shared.Security;
using System.Text.Json;

[ApiController]
[Route("api/ingest")]
public class IngestionController : ControllerBase
{

    private readonly HttpClient _http = new();

    private readonly ScadaDbContext _context;

    private readonly SensorRateLimiter _limiter;

    public IngestionController(ScadaDbContext context, SensorRateLimiter limiter)
    {
        _context = context;

        _limiter = limiter;
    }

    [HttpPost("sensor")]
    public async Task<IActionResult> IngestSensor(
    [FromBody] SecureMessageDto message)
    {

        string json;


        try
        {

            json =
                AesEncryption.Decrypt(
                    message.Data,
                    message.IV);

        }
        catch
        {
            return BadRequest(
                "Invalid encryption");
        }



        if (!EcdsaSignature.Verify(
                json,
                message.Signature))
        {
            return BadRequest(
                "Invalid signature");
        }



        var dto =
            JsonSerializer.Deserialize<SensorIngestDto>(
                json);



        if (dto == null)
        {
            return BadRequest();
        }

        if (!_limiter.Allowed(dto.SensorId))
        {
            await SensorBanningService.BlockSensor(dto.SensorId, _context);

            await EventLogger.LogAsync(
                _context,
                dto.SensorId,
                "SECURITY",
                "Sensor blocked due to excessive requests"
            );

            return StatusCode(
                429,
                "Too many requests");
        }


        var replay =
            await _context.ProcessedMessages
            .AnyAsync(x =>
                x.SensorId == dto.SensorId &&
                x.MessageId == dto.MessageId);



        if (replay)
        {
            return BadRequest(
                "Replay attack detected");
        }



        if (DateTime.UtcNow - dto.SentAt >
            TimeSpan.FromSeconds(30))
        {
            return BadRequest(
                "Old message");
        }


        var sensor =
            await _context.Sensors
            .FirstOrDefaultAsync(
                s => s.Id == dto.SensorId);

        if (sensor == null)
        {
            return BadRequest("Sensor does not exist");
        }

        sensor.LastSeen = DateTime.UtcNow;

        var measurement = new Measurement
        {

            SensorId = dto.SensorId,

            Temperature = dto.Value,

            Timestamp = dto.Timestamp,

            AlarmPriority = dto.AlarmPriority,

            Quality = dto.Quality,

            IsConsensus = false

        };

        _context.Measurements.Add(measurement);

        await _context.SaveChangesAsync();

        _context.ProcessedMessages.Add(
            new ProcessedMessage
            {
                SensorId = dto.SensorId,

                MessageId = dto.MessageId,

                Timestamp = DateTime.UtcNow
            });


        await _context.SaveChangesAsync();

        if (measurement.AlarmPriority != AlarmPriority.None)
        {
            var notification =
                new AlarmNotification
                {
                    SensorId = measurement.SensorId,
                    Value = measurement.Temperature,
                    Priority = measurement.AlarmPriority,
                    Timestamp = measurement.Timestamp
                };

            await _http.PostAsJsonAsync(
                "http://localhost:5068/api/notifications/alarm",
                notification);

            var messageConsole =
                $"ALARM {dto.AlarmPriority}: Temperature {dto.Value:F2}";


            Console.WriteLine(
                $"Sensor {dto.SensorId}: {messageConsole}"
            );


            Console.ResetColor();

            await EventLogger.LogAsync(
                _context,
                dto.SensorId,
                "ALARM",
                messageConsole
            );
        }

        return Ok(
            new IngestResponseDto
            {
                SensorId = measurement.SensorId,

                Value = measurement.Temperature,

                AlarmPriority = measurement.AlarmPriority,

                Quality = measurement.Quality,

                Timestamp = measurement.Timestamp,

                Message = "Measurement accepted!"
            }
        );
    }

    [HttpGet("sensors/active")]
    public async Task<IActionResult> GetActiveSensors()
    {
        var sensors = await _context.Sensors
            .Where(s => s.IsActive && !s.IsBlocked)
            .Select(s => new ActiveSensorDto
            {
                Id = s.Id,

                Name = s.Name,

                MinTemperature = s.MinTemperature,

                MaxTemperature = s.MaxTemperature,

                Alarm1Limit = s.Alarm1Limit,

                Alarm2Limit = s.Alarm2Limit,

                Alarm3Limit = s.Alarm3Limit,

                Quality = s.Quality
            })
            .ToListAsync();

        return Ok(sensors);
    }

    [HttpPost("sensor/{id}/block")]
    public async Task<IActionResult> BlockSensor(int id)
    {

        var success =
            await SensorBanningService
            .BlockSensor(id, _context);


        if (!success)
            return NotFound();


        return Ok(
            $"Sensor {id} blocked for 30 seconds"
        );

    }
}