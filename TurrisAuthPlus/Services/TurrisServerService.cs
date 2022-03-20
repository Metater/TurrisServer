namespace TurrisAuthPlus.Services;

public class TurrisServerService
{
    private TurrisServices services;

    public TurrisServerService(TurrisServices services)
    {
        this.services = services;
    }

    public async Task<string> DeleteAccount(string username, string password)
    {
        return $"{username} {password}";
    }
}