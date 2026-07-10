using ConsensusService;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<ScadaDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHostedService<ConsensusWorker>();


var host = builder.Build();

host.Run();