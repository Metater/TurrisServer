var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuthService>((serviceProvider) => new AuthService());

var app = builder.Build();

app.UseEndpointAuth();
app.UseKeyAuth();

app.MapGet("/hello", () => "Hello, World!");

app.MapGet("/test", (int i) => i);

//app.MapGet("/client/createaccount", async (string gameCode, string username, string password) =>
    //await auth.CreateAccount(gameCode, username, password));

app.Run();