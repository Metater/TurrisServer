namespace TurrisAuthPlus.Services;

public class GameCodesService
{
    private readonly TurrisServices services;

    private readonly object gameCodesLock = new();
    private readonly List<string> gameCodes = new();
    
    public GameCodesService(TurrisServices services)
    {
        this.services = services;
    }

    public bool TryLockGameCode(string gameCode)
    {
        lock (gameCodesLock)
        {
            return gameCodes.Remove(gameCode);
        }
    }

}
