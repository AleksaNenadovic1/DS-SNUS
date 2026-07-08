using ConsensusService;
using IngestionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<ScadaDbContext>(
    options =>
    {
        options.UseNpgsql(
            builder.Configuration
            .GetConnectionString("DefaultConnection")
        );
    });


builder.Services.AddHostedService<ConsensusWorker>();


var host = builder.Build();

host.Run();