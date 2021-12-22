var builder = WebApplication.CreateBuilder();

string serverKey = args[0];
string clientKey = args[1];

object serversLock = new();
List<TurrisServer> servers = new();

object gameCodesLock = new();
List<string> gameCodes = new();

object accountsLock = new();
Dictionary<string, string> accounts = new();

ConcurrentDictionary<string, TurrisServer> serverCache = new();
ConcurrentDictionary<string, TurrisServer> joinCodeCache = new();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/createaccount", async ctx =>
{
    // input:
    // client key
    // game code
    // username
    // password

    // output:
    // result code
});



// auth
app.MapGet("/auth", async ctx =>
{
    // input:
    // client key
    // username
    // pw hash

    // output:
    // result code
    // auth token
});


// auth valid
app.MapGet("/authvalid", async ctx =>
{
    // input:
    // server key
    // auth token

    // output:
    // result code
});




// delete account
app.MapGet("/deleteaccount", async ctx =>
{

});
// input:
// server key
// username

// output:
// result code




// create game code
app.MapGet("/creategamecode", async ctx =>
{
    if (!await Auth(ctx, serverKey)) return;
    string gameCode;
    lock (gameCodesLock)
    {
        gameCode = Guid.NewGuid().ToString();
        gameCodes.Add(gameCode);
    }
    await ctx.Response.WriteAsync(gameCode);
    Save();
});

// output:
// game code


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
        await ctx.Response.WriteAsync("401");
        return false;
    }
    return true;
}

async Task<bool> CheckQuery(HttpContext ctx, string query)
{
    bool has = ctx.Request.Query.ContainsKey(query);
    if (!has) await ctx.Response.WriteAsync("400");
    return has;
}

string GetQuery(HttpContext ctx, string query)
{
    return ctx.Request.Query[query];
}

void Save()
{
    lock (gameCodesLock)
    {

    }
    lock ()
    // game codes
    // accounts: username, pw hash
}

app.Run();