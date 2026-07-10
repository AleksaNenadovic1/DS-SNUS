using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Data;

public class ScadaDbContext : DbContext
{
    public ScadaDbContext(DbContextOptions<ScadaDbContext> options)
        : base(options) { }

    public DbSet<Sensor> Sensors => Set<Sensor>();

    public DbSet<Measurement> Measurements => Set<Measurement>();

    public DbSet<ConsensusValue> ConsensusValues => Set<ConsensusValue>();

    public DbSet<EventLog> EventLogs => Set<EventLog>();

    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

}