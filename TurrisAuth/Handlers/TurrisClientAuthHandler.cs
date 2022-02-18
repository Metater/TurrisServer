namespace TurrisAuth.Handlers;

public class TurrisClientAuthHandler : TurrisAuthHandler
{
    public TurrisClientAuthHandler(WebApplication app, TurrisAuthData data, TurrisValidation validation) : base(app, data, validation)
    {
        app.MapGet("/client/createaccount", CreateAccount);
        app.MapGet("/client/auth", Auth);
        app.MapGet("/client/creategame", CreateGame);
        app.MapGet("/client/joingame", JoinGame);
        app.MapGet("/client/deleteaccount", DeleteAccount);
    }

    async Task CreateAccount(HttpContext ctx)
    {
        if (!await validation.AuthClient(ctx)) return;

        (bool validGamecode, string gameCode) = await validation.GameCode(ctx);
        if (!validGamecode) return;

        (bool validPassword, string password) = await validation.Password(ctx);
        if (!validPassword)
        {
            data.AddGameCode(gameCode);
            return;
        }

        string passwordHash = TurrisUtils.Hash(password);

        (bool validUsername, string username) = await validation.Username(ctx);
        if (!validUsername)
        {
            data.AddGameCode(gameCode);
            return;
        }
        if (validation.UsernameExists(username))
        {
            await ctx.Response.WriteAsync("400\nUsernameExists");
            data.AddGameCode(gameCode);
            return;
        }
        data.AddAccount(username, passwordHash);

        data.QueueSave();

        await ctx.Response.WriteAsync("200");
    }

    async Task Auth(HttpContext ctx)
    {
        if (!await validation.AuthClient(ctx)) return;

        (bool validUsername, string username) = await validation.Username(ctx);
        if (!validUsername) return;

        (bool validPassword, string password) = await validation.Password(ctx);
        if (!validPassword) return;

        string providedPasswordHash = TurrisUtils.Hash(password);

        if (data.GetPasswordHash(username, out string? passwordHash))
        {
            if (providedPasswordHash != passwordHash)
            {
                await ctx.Response.WriteAsync("400\nPasswordInvalid");
                return;
            }
        }
        else
        {
            await ctx.Response.WriteAsync("400\nUsernameNotFound");
            return;
        }

        data.RemovePlayer(username);

        string authToken = Guid.NewGuid().ToString();
        TurrisPlayer authEntry = new(username, authToken, DateTime.Now.AddHours(12));
        data.AddPlayer(authEntry);
        await ctx.Response.WriteAsync($"200\nAuthToken:{authToken}");
    }

    async Task CreateGame(HttpContext ctx)
    {
        if (!await validation.AuthClient(ctx)) return;

        if (!TryGetAuthToken(ctx, out TurrisPlayer? authEntry))
        {
            await ctx.Response.WriteAsync("400\nAuthTokenInvalid");
            return;
        }

        int lowestLoadFactor = int.MaxValue;
        TurrisServer? assigned = null;
        lock (serversLock)
        {
            foreach (TurrisServer server in servers)
            {
                if (server.loadFactor < lowestLoadFactor)
                {
                    assigned = server;
                    lowestLoadFactor = server.loadFactor;
                }
            }
        }

        if (assigned is null)
        {
            await ctx.Response.WriteAsync("400\nNoServerAssigned");
            return;
        }

        authEntry!.serverIntent = assigned.guid;
        authEntry.serverIntentType = ServerIntentType.CreateGame;
        authEntry.serverIntentExpiration = DateTime.Now.AddSeconds(15);

        await ctx.Response.WriteAsync($"200\nEndpoint:{assigned.endpoint}");
    }

    async Task JoinGame(HttpContext ctx)
    {
        if (!await validation.AuthClient(ctx)) return;
    }

    async Task DeleteAccount(HttpContext ctx)
    {
        if (!await validation.AuthClient(ctx)) return;
    }
}