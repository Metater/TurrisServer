namespace TurrisAuthPlus;

public class GameCodesService
{
    private Services services;

    private readonly object gameCodesLock = new();
    private readonly List<string> gameCodes = new();
    
    public GameCodesService(Services services)
    {
        this.services = services;
    }

}
