using IngestionService.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Models;

namespace ConsensusService;


public class ConsensusWorker : BackgroundService
{

    private readonly IServiceProvider _serviceProvider;


    public ConsensusWorker(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }



    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {

            try
            {

                using var scope =
                    _serviceProvider.CreateScope();


                var context =
                    scope.ServiceProvider
                    .GetRequiredService<ScadaDbContext>();


                await CalculateConsensus(context);


            }
            catch (Exception ex)
            {

                Console.WriteLine(
                    $"Consensus error: {ex.Message}"
                );

            }



            // TEST - 10 sekundi
            await Task.Delay(
                TimeSpan.FromSeconds(60),
                stoppingToken
            );

        }

    }





    private async Task CalculateConsensus(
        ScadaDbContext context)
    {


        var from =
            DateTime.UtcNow.AddSeconds(-60);


        var measurements =
            await context.Measurements
            .Where(m =>
                m.Timestamp >= from &&
                m.Sensor.IsActive &&
                !m.Sensor.IsBlocked &&
                m.Sensor.Quality == SensorQuality.GOOD)
            .ToListAsync();



        if (measurements.Count == 0)
        {

            Console.WriteLine(
                "No measurements."
            );

            return;

        }



        Console.WriteLine(
            $"Processing {measurements.Count} measurements"
        );



        // sortiranje vrednosti

        var sorted =
            measurements
            .Select(m => m.Temperature)
            .OrderBy(v => v)
            .ToList();



        // 20% trimmed mean

        int trimCount =
            (int)(sorted.Count * 0.20);



        var trimmed =
            sorted
            .Skip(trimCount)
            .Take(
                sorted.Count - (trimCount * 2)
            )
            .ToList();



        if (trimmed.Count == 0)
        {
            return;
        }



        double trimmedMean =
            trimmed.Average();




        // najbliže realno merenje

        var consensusMeasurement =
            measurements
            .OrderBy(m =>
                Math.Abs(
                    m.Temperature - trimmedMean
                ))
            .First();




        // označi izabrano merenje

        consensusMeasurement.IsConsensus = true;



        // upis u ConsensusValues

        var consensus =
            new ConsensusValue
            {

                Timestamp =
                    DateTime.UtcNow,


                Value =
                    consensusMeasurement.Temperature

            };



        context.ConsensusValues.Add(consensus);



        await context.SaveChangesAsync();



        Console.WriteLine(
            $"Consensus value: {consensus.Value:F2}"
        );

    }

}