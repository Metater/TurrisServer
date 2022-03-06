namespace TurrisAuthPlus;

public class GameCodesService
{
    private readonly object gameCodesLock = new();
    private readonly List<string> gameCodes = new();
}
