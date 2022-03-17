namespace TurrisAuthPlus;

public class AccountsService
{
    private Services services;

    private readonly object accountsLock = new();
    private readonly Dictionary<string, string> accounts = new();
    
    public AccountsService(Services services)
    {
        this.services = services;
    }

    public async Task<IResult> CreateAccount(string gameCode, string username, string password)
    {

        return Results.Ok("Created account");
    }

}