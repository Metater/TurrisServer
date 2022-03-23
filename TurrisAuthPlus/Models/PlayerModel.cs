namespace TurrisAuthPlus.Models;

public record PlayerModel(string Username, string AuthToken, DateTime Expiration)
{
    public string serverIntent = "";
    public ServerIntentType serverIntentType = ServerIntentType.None;
    public DateTime serverIntentExpiration = DateTime.Now;

    public bool IsServerIntent(string serverIntent)
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
        serverIntent = "";
        serverIntentType = ServerIntentType.None;
        serverIntentExpiration = DateTime.Now;
    }

    public enum ServerIntentType
    {
        None,
        CreateGame,
        JoinGame
    }
}