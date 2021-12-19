namespace TurrisServer;

public class Program
{
    public static HttpClient HttpClient = new();

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        string serverKey = File.ReadAllText($"{Directory.GetCurrentDirectory()}/serverKey.secret");
        TurrisDispatcher dispatcher = new(serverKey);
        dispatcher.Start();
        int port = 12733;
        for (int i = 0; i < 10; i++)
        {
            TurrisServer turrisServer = new(dispatcher, 1, port++);
            Thread thread = new(() =>
            {
                StartServer(turrisServer);
            });
            thread.Start();
        }
    }

    private static void StartServer(TurrisServer turrisServer)
    {
        while (true)
        {
            turrisServer.server.ManualReceive();
            turrisServer.Tick();
            turrisServer.server.ManualUpdate((int)turrisServer.timer.ElapsedMilliseconds);
            turrisServer.timer.Restart();
            Thread.Sleep(turrisServer.pollPeriod);
        }
    }
}
