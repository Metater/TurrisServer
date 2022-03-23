namespace TurrisAuthPlus.Services;

public class ServersService
{
    private readonly TurrisServices services;

    private readonly object serversLock = new();
    private readonly List<ServerModel> servers = new();

    private readonly Random random = new();

    public ServersService(TurrisServices services)
    {
        this.services = services;
    }

    public bool TryCreateGame(PlayerModel host, out ServerModel? server, out GameModel? game)
    {
        server = null;
        game = null;
        lock (serversLock)
        {
            int lowestLoadFactor = int.MaxValue;
            foreach (ServerModel s in servers)
            {
                if (s.CalculateLoadFactor() < lowestLoadFactor)
                    server = s;
            }
            if (server is null)
                return false;
            string joinCode = GenerateJoinCode();
            game = server.CreateGame(host, joinCode);
            return true;
        }
    }

    public bool TryJoinGame(PlayerModel player, string joinCode, out GameModel? game)
    {
        game = null;
        lock (serversLock)
        {
            if (player.IsInGame())
                return false;
            ServerModel? server = servers.Find(s => s.ServerId == player.serverIntent);
            if (server is null)
                return false;
            if (!server.TryGetGame(joinCode, out game))
                return false;
            game!.JoinGame(player);
            player.currentGame = joinCode;
            return true;
        }
    }

    private string GenerateJoinCode()
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string joinCode = "";
        while (joinCode == "" || servers.Any(s => s.IsJoinCodeUnique(joinCode)))
        {
            joinCode = new string(Enumerable.Repeat(alphabet, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        return joinCode;
    }
}