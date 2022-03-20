namespace TurrisAuthPlus.Services;

public class AccountsService
{
    private TurrisServices services;

    private readonly object accountsLock = new();
    private readonly Dictionary<string, string> accounts = new();
    
    public AccountsService(TurrisServices services)
    {
        this.services = services;
    }

    public bool TryCreateAccount(string username, string password, out string message)
    {
        lock (accountsLock)
        {
            if (accounts.ContainsKey(username))
                message = "400\nUsernameExists";

        }
    }

}