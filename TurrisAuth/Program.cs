var builder = WebApplication.CreateBuilder();

string serverKey = args[0];
string clientKey = args[1];

object serversLock = new();
List<TurrisServer> servers = new();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/creategame", async ctx =>
{
    if (!await Auth(ctx, clientKey)) return;
    servers.Ran
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
});

app.MapGet("/guid", async ctx =>
{
    await ctx.Response.WriteAsync(Guid.NewGuid().ToString());
});

async Task<bool> Auth(HttpContext ctx, string key)
{
    if (!ctx.Request.Query.ContainsKey("key"))
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsync("400");
        return false;
    }
    if (ctx.Request.Query["key"] != key)
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("401");
        return false;
    }
    return true;
}

app.Run();