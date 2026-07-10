namespace Shared.Configuration;

public static class ServerConfiguration
{
    // Change this when the server PC IP changes
    private const string ServerIp = "192.168.0.46";

    public static string BaseUrl =>
        $"http://{ServerIp}:30080";

    public static string IngestionUrl =>
        $"{BaseUrl}/api/ingest";

    public static string NotificationHubUrl =>
        $"{BaseUrl}/alarmHub";
}
