namespace TurrisAuthPlus.Models;

public record GameModel(PlayerModel Host, string JoinCode)
{
    private readonly List<PlayerModel> players = new();

    public void JoinGame(PlayerModel player)
    {
        players.Add(player);
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }
}