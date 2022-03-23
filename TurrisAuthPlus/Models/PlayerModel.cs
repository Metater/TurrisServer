namespace TurrisAuthPlus.Models;

public record PlayerModel(string Username, string AuthToken, DateTime Expiration)
{
    public string serverIntent = "";
    public ServerIntentType serverIntentType = ServerIntentType.None;
    public DateTime serverIntentExpiration = DateTime.Now;

    public string currentGame = "";

    public bool IsServerIntent(string serverIntent) =>
        this.serverIntent == serverIntent;
    public bool IsServerIntentType(ServerIntentType serverIntentType) => 
        this.serverIntentType == serverIntentType;
    public bool IsServerIntentExpired() =>
        serverIntentExpiration <= DateTime.Now;

    public bool IsInGame() => currentGame == "";

    public void SetServerIntent(string serverId, ServerIntentType serverIntentType)
    {
        serverIntent = serverId;
        this.serverIntentType = serverIntentType;
        serverIntentExpiration = DateTime.Now.AddMinutes(1);
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