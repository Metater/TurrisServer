namespace TurrisAuth;

public class TurrisAuthEntry
{
    public readonly string username;
    public readonly string authToken;
    public readonly DateTime expiration;

    public string serverIntent = "";
    public ServerIntentType serverIntentType = ServerIntentType.None;
    public DateTime serverIntentExpiration = DateTime.Now;

    public TurrisAuthEntry(string username, string authToken, DateTime expiration)
    {
        this.username = username;
        this.authToken = authToken;
        this.expiration = expiration;
    }
}

public enum ServerIntentType
{
    None,
    CreateGame,
    JoinGame
}