var builder = WebApplication.CreateBuilder(args);

Services services = new();

var app = builder.Build();

app.UseEndpointAuth();
app.UseKeyAuth();

app.MapGet("/client/createaccount", async (string gameCode, string username, string password) =>
    await services.accounts.CreateAccount(gameCode, username, password));

app.MapGet("/client/authplayer", async (string username, string password) =>
    await services.players.AuthPlayer(username, password));

app.Run();