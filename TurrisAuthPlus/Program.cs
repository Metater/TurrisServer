var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuthService>((serviceProvider) => new AuthService());

var app = builder.Build();

app.UseExceptionHandler("/hello");

app.Use(async (ctx, next) =>
{
    Console.WriteLine(ctx.Request.Path);
    if (!ctx.Request.Query.ContainsKey("key"))
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsync("401");
        return;
    }
    if (ctx.Request.Path.StartsWithSegments("/client"))
    {
       //ctx.Request.Query.ContainsKey()
    }
    else if (ctx.Request.Path.StartsWithSegments("/server"))
    {

    }
    else
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsync("401");
        return;
    }
    await next(ctx);
});

app.MapGet("/hello", () => "Hello, World!");

app.MapGet("/client/createaccount", async (string gameCode, string username, string password, AuthService auth) =>
    await auth.CreateAccount(gameCode, username, password));

app.Run();