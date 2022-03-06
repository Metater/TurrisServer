var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuthService>((serviceProvider) => new AuthService());

var app = builder.Build();

app.UseEndpointAuth();
app.UseKeyAuth();

/*
if (!ctx.Request.Query.ContainsKey("key"))
{
    ctx.Request.Query.All()
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
*/

app.MapGet("/hello", () => "Hello, World!");

//app.MapGet("/client/createaccount", async (string gameCode, string username, string password) =>
    //await auth.CreateAccount(gameCode, username, password));

app.Run();