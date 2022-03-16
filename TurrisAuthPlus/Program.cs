var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AuthService>((serviceProvider) => new AuthService(serviceProvider));
builder.Services.AddSingleton<GameCodesService>((serviceProvider) => new GameCodesService(serviceProvider));
builder.Services.AddSingleton<AccountsService>((serviceProvider) => new AccountsService(serviceProvider));

var app = builder.Build();

app.UseEndpointAuth();
app.UseKeyAuth();

app.MapGet("/client/createaccount", async (string gameCode, string username, string password) =>
    //await auth.CreateAccount(gameCode, username, password));
    $"{gameCode} {username} {password}");

app.Run();