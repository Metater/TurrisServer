namespace TurrisAuthPlus.Models;

public record ServerModel(string ServerId, string Endpoint)
{
    private readonly List<GameModel> unpolledGames = new();
    private readonly List<GameModel> games = new();

    public GameModel CreateGame(PlayerModel host, string joinCode)
    {
        GameModel game = new(host, joinCode);
        unpolledGames.Add(game);
        return game;
    }

    public bool TryJoinGame(PlayerModel player, string joinCode, out GameModel? game)
    {
        game = games.Find(g => g.JoinCode == joinCode);
        if (game is null)
            return false;
        game.TryJoinGame(player);
        return true;
    }

    public int CalculateLoadFactor()
    {
        // Factor game number in later if needed
        return games.Sum(g => g.GetPlayerCount());
    }

    public bool IsJoinCodeUniqueForServer(string joinCode)
    {
        return (!unpolledGames.Any(g => g.JoinCode == joinCode)) && (!games.Any(g => g.JoinCode == joinCode));
    }
}