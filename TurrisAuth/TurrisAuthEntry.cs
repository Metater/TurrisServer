namespace TurrisAuth;

public class TurrisPlayer
{
    public readonly string username;
    public readonly string authToken;
    public readonly DateTime expiration;

    public string serverIntent = "";
    public ServerIntentType serverIntentType = ServerIntentType.None;
    public DateTime serverIntentExpiration = DateTime.Now;
    
    public bool inGame = false;
    public string game = "";

    public TurrisPlayer(string username, string authToken, DateTime expiration)
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