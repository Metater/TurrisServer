namespace TurrisAuthPlus;

public class AccountsService
{
    private readonly object accountsLock = new();
    private readonly Dictionary<string, string> accounts = new();

    public async Task<IResult> CreateAccount(string gameCode, string username, string password)
    {
        return Results.Ok("Created account");
    }

}