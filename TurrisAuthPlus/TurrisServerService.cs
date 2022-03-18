namespace TurrisAuthPlus;

public class TurrisServerService
{
    public async Task<string> DeleteAccount(string username, string password)
    {
        return $"{username} {password}";
    }
}