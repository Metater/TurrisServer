namespace TurrisAuthPlus;

public class AccountsService
{
    private IServiceProvider serviceProvider;

    private readonly object accountsLock = new();
    private readonly Dictionary<string, string> accounts = new();
    66
    public AccountsService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<IResult> CreateAccount(string gameCode, string username, string password)
    {

        return Results.Ok("Created account");
    }

}