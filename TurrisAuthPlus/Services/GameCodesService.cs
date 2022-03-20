namespace TurrisAuthPlus.Services;

public class GameCodesService
{
    private TurrisServices services;

    private readonly object gameCodesLock = new();
    private readonly List<string> gameCodes = new();
    
    public GameCodesService(TurrisServices services)
    {
        this.services = services;
    }

}
