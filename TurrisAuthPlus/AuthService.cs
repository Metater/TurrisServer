namespace TurrisAuthPlus;

public class AuthService
{
    public AuthService()
    {
        Console.WriteLine("Ctor");
    }

    public async Task<IResult> CreateAccount(string gameCode, string username, string password)
    {
        return Results.Ok("Created account");
    }
}
