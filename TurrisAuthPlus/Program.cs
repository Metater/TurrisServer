var builder = WebApplication.CreateBuilder(args);

Services services = new();

var app = builder.Build();

app.UseEndpointAuth();
app.UseKeyAuth();

app.MapGet("/client/createaccount", async (string gameCode, string username, string password) =>
    await services.auth.CreateAccount(gameCode, username, password));

app.Run();