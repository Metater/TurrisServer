namespace TurrisAuthPlus;

public class GameCodesService
{
    private IServiceProvider serviceProvider;

    private readonly object gameCodesLock = new();
    private readonly List<string> gameCodes = new();
    
    public GameCodesService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

}
