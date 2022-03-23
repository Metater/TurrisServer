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

    public bool TryGetGame(string joinCode, out GameModel? game)
    {
        game = games.Find(g => g.JoinCode == joinCode);
        return game is not null;
    }

    public int CalculateLoadFactor()
    {
        // Factor game number in later if needed
        return games.Sum(g => g.GetPlayerCount());
    }

    public bool IsJoinCodeUnique(string joinCode)
    {
        return (!unpolledGames.Any(g => g.JoinCode == joinCode)) && (!games.Any(g => g.JoinCode == joinCode));
    }
}