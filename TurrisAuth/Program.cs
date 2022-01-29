var builder = WebApplication.CreateBuilder();

string serverKey = File.ReadAllText(Directory.GetCurrentDirectory() + "/serverKey.secret");
string clientKey = File.ReadAllText(Directory.GetCurrentDirectory() + "/clientKey.secret");

string gameCodesPath = Directory.GetCurrentDirectory() + "/gameCodes.turris";
string accountsPath = Directory.GetCurrentDirectory() + "/accounts.turris";

object gameCodesLock = new();
List<string> gameCodes = new();
bool saveGameCodes = false;

object accountsLock = new();
Dictionary<string, string> accounts = new();
bool saveAccounts = false;

object serversLock = new();
List<TurrisServer> servers = new();
// server guid | join code, server
ConcurrentDictionary<string, TurrisServer> serverCache = new();

object authLock = new();
List<TurrisPlayer> players = new();
// username | authToken, player
ConcurrentDictionary<string, TurrisPlayer> playerCache = new();

await Load();

var app = builder.Build();

//app.UseHttpsRedirection();

app.MapGet("/createaccount", async ctx =>
{
    if (!await Auth(ctx, clientKey)) return;
    if (!await CheckQuery(ctx, "gamecode")) return;
    if (!await CheckQuery(ctx, "username")) return;
    if (!await CheckQuery(ctx, "password")) return;

    // verify game code
    string gameCode = GetQuery(ctx, "gamecode");
    bool gameCodeValid;
    lock (gameCodesLock)
    {
        gameCodeValid = gameCodes.Remove(gameCode);
    }
    if (!gameCodeValid)
    {
        await ctx.Response.WriteAsync("400\nInvalidGameCode");
        return;
    }

    // verify password
    string password = GetQuery(ctx, "password");
    if (password.Length < 6)
    {
        AddGameCode();
        await ctx.Response.WriteAsync("400\nPasswordTooShort");
        return;
    }
    string passwordHash = Hash(password);

    // verify username
    string username = GetQuery(ctx, "username");
    if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$") || username.Length > 16 || username.Length < 1)
    {
        AddGameCode();
        await ctx.Response.WriteAsync("400\nUsernameInvalid");
        return;
    }
    bool usernameUnique;
    lock (accountsLock)
    {
        usernameUnique = !accounts.ContainsKey(username);
        if (usernameUnique)
        {
            accounts.Add(username, passwordHash);
        }
    }
    if (!usernameUnique)
    {
        AddGameCode();
        await ctx.Response.WriteAsync("400\nUsernameExists");
        return;
    }

    void AddGameCode()
    {
        lock (gameCodesLock)
        {
            gameCodes.Add(gameCode);
        }
    }

    saveGameCodes = true;
    saveAccounts = true;

    await ctx.Response.WriteAsync("200");
});

app.MapGet("/deleteaccount", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
    if (!await CheckQuery(ctx, "username")) return;

    string username = GetQuery(ctx, "username");
    bool deleted = false;
    lock (accountsLock)
    {
        deleted = accounts.Remove(username);
    }
    if (deleted)
    {
        if (playerCache.TryRemove(username, out TurrisPlayer? authEntry))
        {
            playerCache.TryRemove(authEntry.authToken, out _);
            lock (authLock)
            {
                players.Remove(authEntry);
            }
        }
        saveAccounts = true;
        await ctx.Response.WriteAsync("200");
    }
    else
        await ctx.Response.WriteAsync("400\nUsernameNotFound");
});

app.MapGet("/auth", async ctx =>
{
    if (!await Auth(ctx, clientKey)) return;
    if (!await CheckQuery(ctx, "username")) return;
    if (!await CheckQuery(ctx, "password")) return;

    string username = GetQuery(ctx, "username");
    if (playerCache.TryRemove(username, out TurrisPlayer? staleAuthEntry))
    {
        playerCache.TryRemove(staleAuthEntry.authToken, out _);
        lock (authLock)
        {
            players.Remove(staleAuthEntry);
        }
    }

    string? passwordHash;
    bool usernameExists;
    lock (accountsLock)
    {
        usernameExists = accounts.TryGetValue(username, out passwordHash);
    }
    if (!usernameExists)
    {
        await ctx.Response.WriteAsync("400\nUsernameNotFound");
        return;
    }

    string providedPasswordHash = Hash(GetQuery(ctx, "password"));
    if (providedPasswordHash != passwordHash!)
    {
        await ctx.Response.WriteAsync("400\nPasswordInvalid");
        return;
    }

    string authToken = Guid.NewGuid().ToString();
    TurrisPlayer authEntry = new(username, authToken, DateTime.Now.AddHours(12));
    lock (authLock)
    {
        players.Add(authEntry);
    }
    playerCache.TryAdd(username, authEntry);
    playerCache.TryAdd(authToken, authEntry);
    await ctx.Response.WriteAsync($"200\nAuthToken:{authToken}");
});


app.MapGet("/intentvalid", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
    if (!await CheckQuery(ctx, "authToken")) return;
    if (!await CheckQuery(ctx, "serverId")) return;
    if (!await CheckQuery(ctx, "serverIntentType")) return;

    if (!TryGetAuthToken(ctx, out TurrisPlayer? authEntry))
    {
        await ctx.Response.WriteAsync("400\nAuthTokenInvalid");
        return;
    }
    if (DateTime.Now >= authEntry!.serverIntentExpiration || authEntry.serverIntent != GetQuery(ctx, "serverId") || !int.TryParse(GetQuery(ctx, "serverIntentType"), out int serverIntentType) || serverIntentType != ((int)authEntry.serverIntentType))
    {
        authEntry.serverIntent = "";
        authEntry.serverIntentType = ServerIntentType.None;
        authEntry.serverIntentExpiration = DateTime.Now;
        await ctx.Response.WriteAsync("400\nServerIntentInvalid");
        return;
    }
    authEntry.serverIntent = "";
    authEntry.serverIntentType = ServerIntentType.None;
    authEntry.serverIntentExpiration = DateTime.Now;
    await ctx.Response.WriteAsync($"200\nUsername:{authEntry.username}");
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

async Task<bool> Auth(HttpContext ctx, string key)
{
    if (!await CheckQuery(ctx, "key"))
    {
        return false;
    }
    if (GetQuery(ctx, "key") != key)
    {
        await ctx.Response.WriteAsync("400\nInvalidKey");
        return false;
    }
    return true;
}

async Task<bool> CheckQuery(HttpContext ctx, string query)
{
    bool has = ctx.Request.Query.ContainsKey(query);
    if (!has) await ctx.Response.WriteAsync($"400\nMissingQuery:{query}");
    return has;
}

bool TryGetAuthToken(HttpContext ctx, out TurrisPlayer? authEntry)
{
    return playerCache.TryGetValue(GetQuery(ctx, "authToken"), out authEntry);
}

string GetQuery(HttpContext ctx, string query)
{
    return ctx.Request.Query[query];
}

string Hash(string str)
{
    HashAlgorithm sha = SHA256.Create();
    byte[] result = sha.ComputeHash(Encoding.ASCII.GetBytes(str));
    return Convert.ToBase64String(result);
}

async Task Load()
{
    if (File.Exists(gameCodesPath))
    {
        gameCodes = new(await File.ReadAllLinesAsync(gameCodesPath));
    }
    if (File.Exists(accountsPath))
    {
        string[] accountLines = await File.ReadAllLinesAsync(accountsPath);
        foreach (string accountLine in accountLines)
        {
            string[] account = accountLine.Split(':');
            accounts.Add(account[0], account[1]);
        }
    }
}

async Task Save()
{
    List<Task> savingFiles = new();
    if (saveGameCodes)
    {
        saveGameCodes = false;
        List<string> gameCodesCopy;
        lock (gameCodesLock)
        {
            gameCodesCopy = new(gameCodes);
        }
        savingFiles.Add(File.WriteAllLinesAsync(gameCodesPath, gameCodesCopy));
    }
    if (saveAccounts)
    {
        saveAccounts = false;
        List<KeyValuePair<string, string>> accountsCopy;
        lock (accountsLock)
        {
            accountsCopy = new(accounts);
        }
        List<string> accountLines = new();
        foreach ((string username, string passwordHash) in accountsCopy)
        {
            accountLines.Add(username + ":" + passwordHash);
        }
        savingFiles.Add(File.WriteAllLinesAsync(accountsPath, accountLines));
    }
    await Task.WhenAll(savingFiles);
}

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