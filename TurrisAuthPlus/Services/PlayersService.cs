namespace TurrisAuthPlus.Services;

public class PlayersService
{
    private TurrisServices services;

    private readonly object playersLock = new();
    private readonly List<PlayerModel> players = new();

    public PlayersService(TurrisServices services)
    {
        this.services = services;
    }

    public PlayerModel GetRenewedPlayer(string username)
    {
        lock (playersLock)
        {
            players.RemoveAll(p => p.Username == username);
            PlayerModel player = new(username, Guid.NewGuid().ToString(), DateTime.Now.AddHours(12));
            players.Add(player);
            return player;
        }
    }

    public bool PlayerValid(string authToken, Action<PlayerModel>? validAction = null)
    {
        lock (playersLock)
        {
            PlayerModel player = players.Find(p => p.AuthToken == authToken)!;
            if (player is not null)
            {
                validAction?.Invoke(player);
                return true;
            }
        }
        return false;
    }
}