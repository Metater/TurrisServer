namespace TurrisAuthPlus;

public enum ServerIntentType
{
    None,
    CreateGame,
    JoinGame
}

public record Player(string Username, Guid AuthToken, DateTime Expiration)
{
    public Guid serverIntent = Guid.Empty;
    public ServerIntentType serverIntentType = ServerIntentType.None;
    public DateTime serverIntentExpiration = DateTime.Now;

    public bool IsServerIntent(Guid serverIntent)
    {
        return this.serverIntent == serverIntent;
    }
    public bool IsServerIntentType(ServerIntentType serverIntentType)
    {
        return this.serverIntentType == serverIntentType;
    }
    public bool IsServerIntentExpired()
    {
        return serverIntentExpiration <= DateTime.Now;
    }

    public void ResetServerIntent()
    {
        serverIntent = Guid.Empty;
        serverIntentType = ServerIntentType.None;
        serverIntentExpiration = DateTime.Now;
    }
}