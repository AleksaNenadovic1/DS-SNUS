using System.Collections.Concurrent;


namespace IngestionService;


public class SensorRateLimiter
{

    private readonly ConcurrentDictionary<int, List<DateTime>>
        requests = new();



    public bool Allowed(int sensorId)
    {

        var now =
            DateTime.UtcNow;


        var list =
            requests.GetOrAdd(
                sensorId,
                _ => new List<DateTime>());



        lock (list)
        {

            list.RemoveAll(
                x => now - x >
                TimeSpan.FromSeconds(1));



            list.Add(now);



            return list.Count <= 10;

        }

    }

}