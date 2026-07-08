using IngestionService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Dto;
using Shared.Enums;
using Shared.Models;

[ApiController]
[Route("api/ingest")]
public class IngestionController : ControllerBase
{

    private readonly ScadaDbContext _context;

    public IngestionController(ScadaDbContext context)
    {
        _context = context;
    }

    [HttpPost("sensor")]
    public async Task<IActionResult> IngestSensor(
    [FromBody] SensorIngestDto dto)
    {

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

        if (dto.AlarmPriority != AlarmPriority.None)
        {
            Console.ForegroundColor =
                dto.AlarmPriority switch
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


            var message =
                $"ALARM {dto.AlarmPriority}: Temperature {dto.Value:F2}";


            Console.WriteLine(
                $"Sensor {dto.SensorId}: {message}"
            );


            Console.ResetColor();

            await EventLogger.LogAsync(
                _context,
                dto.SensorId,
                "ALARM",
                message
            );
        }

        return Ok();
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
        var sensor =
            await _context.Sensors
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sensor == null)
            return NotFound();


        sensor.IsBlocked = true;

        sensor.IsActive = false;

        sensor.BlockedUntil =
            DateTime.UtcNow.AddSeconds(30);


        await _context.SaveChangesAsync();


        return Ok(
            $"Sensor {id} blocked for 30 seconds"
        );
    }
}