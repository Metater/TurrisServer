var builder = WebApplication.CreateBuilder(args);

TurrisServices services = new();

var app = builder.Build();

app.UseEndpointAuth();
app.UseKeyAuth();

// check for players expiration when creating games and joining games or taking any authToken inputs

// check periodically for players that are expired and are not in any servers, ONLY condition for removal

app.MapGet("/client/createaccount", (string gameCode, string username, string password) =>
    services.turrisClient.CreateAccount(gameCode, username, password));

app.MapGet("/client/deleteaccount", (string username, string password) =>
    services.turrisClient.DeleteAccount(username, password));

app.MapGet("/client/authplayer", (string username, string password) =>
    services.turrisClient.AuthPlayer(username, password));



app.MapGet("/server/deleteaccount", (string username) =>
    services.turrisServer.DeleteAccount(username));

app.Run();