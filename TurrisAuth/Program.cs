var builder = WebApplication.CreateBuilder();

string serverKey = args[0];
string clientKey = args[1];

object gameCodesLock = new();
List<string> gameCodes = new();

object accountsLock = new();
Dictionary<string, string> accounts = new();

object serversLock = new();
List<TurrisServer> servers = new();
// server guid | join code, server
ConcurrentDictionary<string, TurrisServer> serverCache = new();

object authLock = new();
List<TurrisAuthEntry> auth = new();
// username | authToken, auth
ConcurrentDictionary<string, TurrisAuthEntry> authCache = new();

bool save = false;

var app = builder.Build();

app.UseHttpsRedirection();

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
    if (password.Length < 8)
    {
        AddGameCode();
        await ctx.Response.WriteAsync("400\nPasswordTooShort");
        return;
    }
    string passwordHash = Hash(password);

    // verify username
    string username = GetQuery(ctx, "username");
    if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
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

    save = true;

    await ctx.Response.WriteAsync("200");
});

app.MapGet("/deleteaccount", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
    if (!await CheckQuery(ctx, "username")) return;

    bool deleted = false;
    lock (accountsLock)
    {
        deleted = accounts.Remove(GetQuery(ctx, "username"));
    }
    if (deleted) save = true;

    await ctx.Response.WriteAsync("200");
});



// auth
app.MapGet("/auth", async ctx =>
{
    // input:
    // client key
    // username
    // password
    if (!await Auth(ctx, clientKey)) return;
    if (!await CheckQuery(ctx, "username")) return;
    if (!await CheckQuery(ctx, "password")) return;

    string? passwordHash;
    bool usernameValid;
    lock (accountsLock)
    {
        usernameValid = accounts.TryGetValue(GetQuery(ctx, "username"), out passwordHash);
    }
    if (!usernameValid)
    {
        await ctx.Response.WriteAsync("400\nUsernameInvalid");
        return;
    }

    string providedPasswordHash = Hash(GetQuery(ctx, "password"));
    if (providedPasswordHash != passwordHash!)
    {
        await ctx.Response.WriteAsync("400\nPasswordInvalid");
        return;
    }

    string authToken = Guid.NewGuid().ToString();

    await ctx.Response.WriteAsync($"200\nAuthToken:{authToken}");
});


// auth valid
app.MapGet("/authvalid", async ctx =>
{
    // input:
    // server key
    // auth token

    // output:
    // result code
    // username
});






// create game code
app.MapGet("/creategamecode", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
    string gameCode = Guid.NewGuid().ToString();
    lock (gameCodesLock)
    {
        gameCodes.Add(gameCode);
    }
    await ctx.Response.WriteAsync($"200\nGameCode:{gameCode}");
    save = true;
});

// output:
// game code

// list game codes


app.MapGet("/creategame", async ctx =>
{
    if (!await Auth(ctx, clientKey)) return;

    lock (serversLock)
    {

    }
});

app.MapGet("/joingame", async ctx =>
{
    if (!await Auth(ctx, clientKey)) return;
});




app.MapPost("/update", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
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

void Save()
{
    if (!save) return;
    lock (gameCodesLock)
    {

    }
    lock (accountsLock)
    {

    }
    // game codes
    // accounts: username, pw hash
}

List<Task> tasks = new();
tasks.Add(Task.Run(async () =>
{
    while (true)
    {
        Save();
        await Task.Delay(10000);
    }
}));
tasks.Add(app.RunAsync());
await Task.WhenAll(tasks);