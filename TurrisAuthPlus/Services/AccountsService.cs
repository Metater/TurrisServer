namespace TurrisAuthPlus.Services;

public class AccountsService
{
    private TurrisServices services;

    private readonly object accountsLock = new();
    private readonly Dictionary<string, string> accounts = new();

    // TODO add saving to drive
    
    public AccountsService(TurrisServices services)
    {
        this.services = services;
    }

    public bool TryCreateAccount(string username, string password)
    {
        lock (accountsLock)
        {
            if (accounts.ContainsKey(username))
            {
                return false;
            }
            accounts.Add(username, TurrisUtils.Hash(password));
        }
        return true;
    }

    public void DeleteAccount(string username)
    {
        lock (accountsLock)
        {
            accounts.Remove(username);
        }
    }

    public bool AccountValid(string username, string password)
    {
        lock (accountsLock)
        {
            if (!accounts.TryGetValue(username, out string? actualHash))
                return false;
            return actualHash! == TurrisUtils.Hash(password);
        }
    }
}