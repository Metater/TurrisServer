var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/creategame", async ctx =>
{

});

app.MapGet("/joingame", async ctx =>
{

});

app.MapGet("/updateloadfactor", async ctx =>
{

});

app.Run();