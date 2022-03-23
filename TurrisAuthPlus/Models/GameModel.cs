namespace TurrisAuthPlus.Models;

public record GameModel(PlayerModel Host, string JoinCode)
{
    private readonly List<PlayerModel> players = new();

    public bool TryJoinGame(PlayerModel player)
    {
        if (players.Contains(player))
            return false;
        players.Add(player);
        return true;
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }
}