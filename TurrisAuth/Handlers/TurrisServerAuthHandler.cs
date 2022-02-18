namespace TurrisAuth.Handlers;

public class TurrisServerAuthHandler : TurrisAuthHandler
{
    public TurrisServerAuthHandler(WebApplication app, TurrisAuthData data, TurrisValidation validation) : base(app, data, validation)
    {
        app.MapGet("/server/startup", Startup);
        app.MapGet("/server/shutdown", Shutdown);
        app.MapGet("/server/update", Update);
        app.MapGet("/server/poll", Poll);
        app.MapGet("/server/intentvalid", IntentValid);
        app.MapGet("/server/creategamecode", CreateGameCode);
        app.MapGet("/server/listgamecodes", ListGameCodes);
        app.MapGet("/server/deleteaccount", DeleteAccount);
    }

    async Task Startup(HttpContext ctx)
    {

    }

    async Task Shutdown(HttpContext ctx)
    {

    }

    async Task Update(HttpContext ctx)
    {

    }

    async Task Poll(HttpContext ctx)
    {

    }

    async Task IntentValid(HttpContext ctx)
    {

    }

    async Task CreateGameCode(HttpContext ctx)
    {

    }

    async Task ListGameCodes(HttpContext ctx)
    {

    }

    async Task DeleteAccount(HttpContext ctx)
    {

    }
}