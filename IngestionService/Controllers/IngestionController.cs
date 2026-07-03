using IngestionService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Dto;

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
    public async Task<IActionResult> IngestSensor([FromBody] SensorIngestDto dto)
    {
        var measurement = new Measurement
        {
            SensorId = dto.SensorId,
            Temperature = dto.Value,
            Timestamp = dto.Timestamp
        };

        var sensorExists = await _context.Sensors.AnyAsync(s => s.Id == dto.SensorId);

        if (!sensorExists)
            return BadRequest("Sensor does not exist");

        _context.Measurements.Add(measurement);
        await _context.SaveChangesAsync();

        return Ok();
    }
}