var builder = WebApplication.CreateBuilder();

TurrisAuthData data = new();
await data.Load();
TurrisValidation validation = new(data);

WebApplication app = builder.Build();

TurrisClientAuthHandler clientAuth = new(app, data, validation);
TurrisClientAuthHandler serverAuth = new(app, data, validation);

//app.UseHttpsRedirection();

app.MapGet("/deleteaccount", async ctx =>
{
    if (!await validation.AuthClient(ctx)) return;

    (bool validUsername, string username) = await validation.Username(ctx);
    if (!validUsername) return;
    if (!validation.UsernameExists(username))
    {
        await ctx.Response.WriteAsync("400\nUsernameNotFound");
        return;
    }

    data.RemoveAccount(username);

    data.QueueAccountsSave();
    data.RemovePlayer(username);

    await ctx.Response.WriteAsync("200");
});


app.MapGet("/intentvalid", async ctx =>
{
    if (!await validation.AuthServer(ctx)) return;

    await validation.Intent(ctx);
});






app.MapGet("/creategamecode", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
    string gameCode = Guid.NewGuid().ToString();
    lock (gameCodesLock)
    {
        gameCodes.Add(gameCode);
    }
    saveGameCodes = true;
    await ctx.Response.WriteAsync($"200\nGameCode:{gameCode}");
});

app.MapGet("/listgamecodes", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
    string gameCodesList = "";
    lock (gameCodesLock)
    {
        gameCodesList = string.Join(',', gameCodesList);
    }
    await ctx.Response.WriteAsync($"200\nGameCodes:{gameCodesList}");
});


app.MapGet("/creategame", async ctx =>
{
    if (!await Auth(ctx, clientKey)) return;
    if (!await CheckQuery(ctx, "authToken")) return;

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
});

app.MapGet("/joingame", async ctx =>
{
    if (!await Auth(ctx, clientKey)) return;
    if (!await CheckQuery(ctx, "authToken")) return;
    if (!await CheckQuery(ctx, "joinCode")) return;

    if (!TryGetAuthToken(ctx, out TurrisPlayer? authEntry))
    {
        await ctx.Response.WriteAsync("400\nAuthTokenInvalid");
        return;
    }

    string joinCode = GetQuery(ctx, "joinCode");
    if (!serverCache.TryGetValue(joinCode, out TurrisServer? server))
    {
        await ctx.Response.WriteAsync("400\nJoinCodeInvalid");
        return;
    }

    authEntry!.serverIntent = server.guid;
    authEntry.serverIntentType = ServerIntentType.JoinGame;
    authEntry.serverIntentExpiration = DateTime.Now.AddSeconds(15);

    await ctx.Response.WriteAsync($"200\nEndpoint:{server.endpoint}");
});

app.MapGet("/startup", async ctx =>
{

});

app.MapGet("/shutdown", async ctx =>
{

});



app.MapPost("/update", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;

    // new game join code

    // server id
});

app.MapGet("/poll", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;

    // input:
    // server id
});

app.MapGet("/guid", async ctx =>
{
    await ctx.Response.WriteAsync(Guid.NewGuid().ToString());
});

List<Task> tasks = new();
tasks.Add(Task.Run(async () =>
{
    while (true)
    {
        for (int i = 0; i < 100; i++)
        {
            await Task.Delay(10000);
            await Save();
        }
        lock (authLock)
        {
            DateTime now = DateTime.Now;
            players.FindAll(e => now >= e.expiration).ForEach(expiredPlayer =>
            {
                players.Remove(expiredPlayer);
                playerCache.TryRemove(expiredPlayer.username, out _);
                playerCache.TryRemove(expiredPlayer.authToken, out _);
            });
        }
    }
}));
tasks.Add(app.RunAsync());
await Task.WhenAll(tasks);
saveGameCodes = true;
saveAccounts = true;
await Save();